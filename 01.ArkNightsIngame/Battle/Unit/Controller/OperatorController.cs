using System;
using System.Collections.Generic;
using System.Threading;
using Bam.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(OperatorUIHandler))]
public class OperatorController : MonoBehaviour
{
	private Operator op;
	private OperatorUIHandler uiHandler;
	[SerializeField] private StateMachine stateMachine;
	private Animator animator;

	private HashSet<Unit> targetList;
	private Transform objBody;

	private SkinnedMeshRenderer skinnedMeshRenderer;
	private MaterialPropertyBlock mpb;
	private CancellationTokenSource hitToken;

	private float atkCoolTime;
	private bool isAttacking;
	private bool isDeployed;

	private readonly int SKILL_INDEX_ANIM_HASH = Animator.StringToHash("SkillIndex");
	private readonly int ATTACK_ANIM_HASH = Animator.StringToHash("Attack");
	private readonly int ATTACKSPEED_ANIM_HASH = Animator.StringToHash("AttackSpeed");
	private readonly int SKILL_ANIM_HASH = Animator.StringToHash("Skill");

	private readonly int SKILL_ANIM_LOOP1_HASH = Animator.StringToHash("IsLoop_1");
	private readonly int SKILL_ANIM_LOOP2_HASH = Animator.StringToHash("IsLoop_2");
	private readonly int SKILL_ANIM_LOOP3_HASH = Animator.StringToHash("IsLoop_3");

	public event Action<float> OnSkillSpCharge;
	
	public Operator Op => op;
	public Transform Body => objBody;
	public float AtkCoolTime => atkCoolTime;
	public HashSet<Unit> TargetList => targetList;

	private void Awake()
	{
		op = GetComponent<Operator>();
		op.SetController(this);
		uiHandler = GetComponent<OperatorUIHandler>();
		animator = GetComponent<Animator>();
		skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
		mpb = new MaterialPropertyBlock();

		stateMachine = new StateMachine();
		targetList = new HashSet<Unit>();
	}
	private void OnEnable()
	{
		TimeManager.Instance.OnTimeScaleChanged += OnTimeScaleChanged;
	}

	private void OnDisable()
	{
		TimeManager.Instance.OnTimeScaleChanged -= OnTimeScaleChanged;
	}

	private void OnTimeScaleChanged(float newScale)
	{
		animator.speed = newScale;
	}
	
	public void Init()
	{
		objBody = transform.Find("Body");
		op.Init();

		var skill = op.GetSkill();
		LazyAnimatorInit(skill).Forget();
		
		op.OnDeath += () =>
		{
			isDeployed = false;
			uiHandler.UnChainEvent(op.Attribute, skill);
			uiHandler.HpBar.SetSliderActive(false);
			uiHandler.SpBar.SetSliderActive(false);
		};

		op.onHit += () =>
		{
			HitAnimation().Forget();
		};

		//배치 상태
		stateMachine.AddState((int)Operator_State.Deploy, new State().OnEnter(() =>
		{
			op.onDeploy.Invoke();

		}).AddCondition((int)Operator_State.Idle, () => true));

		// 대기 상태
		stateMachine.AddState((int)Operator_State.Idle, new State().OnEnter(() =>
		{

		}).OnStay(() =>
		{
			op.GetTargets(targetList);
		}).AddCondition((int)Operator_State.Attack, () =>
		{
			return targetList.Count > 0;
		}).AddCondition((int)Operator_State.Skill, () =>
		{
			return skill != null && skill.CanUse() && skill.ActiveType == SkillActiveType.Auto;

		}));

		//공격 상태
		stateMachine.AddState((int)Operator_State.Attack, new State().OnEnter(() =>
			{


			}).OnStay(() =>
			{
				if (atkCoolTime > 0 || isAttacking) return;
				op.GetTargets(targetList);

				if (targetList.Count > 0)
				{
					isAttacking = true;
					animator.SetTrigger(ATTACK_ANIM_HASH);
				}
			}).AddCondition((int)Operator_State.Idle, () => targetList.Count == 0)
			.AddCondition((int)Operator_State.Skill, () =>
			{
				return skill != null && skill.CanUse() && skill.ActiveType == SkillActiveType.Auto;

			}).OnEnd(() =>
			{
				isAttacking = false;
			}));

		//스킬 상태
		stateMachine.AddState((int)Operator_State.Skill, new State().OnEnter(() =>
			{
				skill.SetSkill();
				if (skill is IRangeModifyingSkill rangeSkill)
					op.ChangeAttackRange(rangeSkill.RangeID);

				uiHandler.SpBar.ToggleSliderBars(true);
			}).OnStay(() =>
			{
				op.GetTargets(targetList);
				if (skill.UpdateSkill(targetList))
				{
					animator.SetTrigger(SKILL_ANIM_HASH);
				}
			}).AddCondition((int)Operator_State.Idle, () => skill.IsSkillEnd())
			.OnEnd(() =>
			{
				if (skill is IRangeModifyingSkill)
					op.RevertAttackRange();

				uiHandler.SpBar.ToggleSliderBars(false);
			}));
	}
	private async UniTaskVoid LazyAnimatorInit(Skill skill)
	{
		if (skill is null)
			return;
		
		//애니메이터의 초기화가 될때까지 대기
		await UniTask.WaitUntil(() => animator.isInitialized);
		animator.SetInteger(SKILL_INDEX_ANIM_HASH, skill.Index);
	}
	private void Update()
	{
		if (op.IsDead()) return;
		if (!isDeployed) return;
		
		op.Tick();
		UpdateCoolTime();
		stateMachine.Update();
		SetAnimationSpeed();
	}

	public void DeployOperator()
	{
		AutoBattleManager.Instance.RecordEvent(new TimelineEvent(op.OperatorID, TimeManager.Instance.Frame, ReplayEventType.Deploy, new DeployEventData((int)op.Direction, op.OnTile.TileIndex)));

		isDeployed = true;
		isAttacking = false;
		atkCoolTime = 0;

		uiHandler.Init(op.Attribute, op.GetSkill());

		// 배치 오퍼레이터 리스트에 추가
		GameManager.Instance.DeployOperator(op);
		stateMachine.ChangeState((int)Operator_State.Deploy, true);
	}

	public void UseSkillManually()
	{
		stateMachine.ChangeState((int)Operator_State.Skill);
	}

	public void Retreat()
	{
		op.onRetreat.Invoke();
	}

	private void OnAttack()
	{
		op.Attack(targetList);
		SetAtkCoolTime();
	}

	private void AttackEnd()
	{
		isAttacking = false;
	}

	private void OnSkill()
	{
		op.GetSkill()?.Use(op, targetList);
	}
	private void SkillEnd()
	{
		op.GetSkill()?.EndSkillAnim();
	}
	public void SetAtkCoolTime()
	{
		atkCoolTime = GetAtkSpeed();
	}
	private async UniTaskVoid HitAnimation()
	{
		hitToken?.Cancel();
		hitToken = new CancellationTokenSource();
		var token = hitToken.Token;

		Extensions.ChangeMeshColor(skinnedMeshRenderer, mpb, Color.red, "_Color");

		float elapsed = 0f;
		Color from = Color.red;
		Color to = Color.white;

		try
		{
			while (elapsed < 0.5f)
			{
				if (token.IsCancellationRequested) return;

				elapsed += Time.deltaTime;
				float t = Mathf.Clamp01(elapsed / 0.5f);
				Color lerp = Color.Lerp(from, to, t);

				Extensions.ChangeMeshColor(skinnedMeshRenderer, mpb, lerp, "_Color");

				await UniTask.Yield(PlayerLoopTiming.Update, token); // 프레임마다 기다림
			}
		}
		catch (OperationCanceledException)
		{
			return;
		}

		Extensions.ChangeMeshColor(skinnedMeshRenderer, mpb, to, "_Color"); // 마지막 색 보정
	}

	private void UpdateCoolTime()
	{
		if (atkCoolTime > 0)
		{
			atkCoolTime -= Time.deltaTime;

			if (atkCoolTime <= 0)
				atkCoolTime = 0;
		}

		OnSkillSpCharge?.Invoke(Time.deltaTime);
	}

	public float GetAtkSpeed(float atkSpeedDelta = 0)
	{
		return CombatFormulaUtil.CalculateAttackInterval(op.Attribute.AttackSpeed, op.Attribute.GetAddTotalExtraAttribute(AttributeType.AtkSpeed), atkSpeedDelta);
	}

	public void SetAnimationSpeed(float atkSpeedDelta = 0)
	{
		float bonus = op.Attribute.GetAddTotalExtraAttribute(AttributeType.AtkSpeed);
		float speedScale = 1f + bonus - atkSpeedDelta;

		animator.SetFloat(ATTACKSPEED_ANIM_HASH, speedScale);
	}

	public float GetAnimActualLength(string animationName)
	{
		return animator.GetAnimationActualLength(animationName, ATTACKSPEED_ANIM_HASH);
	}

	public void SetAnimationLoop(int skillIndex, bool isLoop)
	{
		var hash = skillIndex switch
		{
			0 => SKILL_ANIM_LOOP1_HASH,
			1 => SKILL_ANIM_LOOP2_HASH,
			2 => SKILL_ANIM_LOOP3_HASH
		};

		animator.SetBool(hash, isLoop);
	}
}