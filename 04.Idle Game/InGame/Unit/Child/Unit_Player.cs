using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class Unit_Player : UnitBase
{
	[SerializeField] private Animator[] petAnimators;
	[SerializeField] private SpriteRenderer[] petSpriteRenderers;
	
	private Player player;
	
	public override void Init(UnitAI unitAI)
	{
		this.unitAI = unitAI;
		player = Player.Instance;
        
		double hp = player.GetTotalHp();
		double atkSpeed = player.StatDic[StatType.AtkSpeed].effect.Value;

		unitAI.MyHp.Init(hp);
		Stat = new UnitStat(0, 2, (float)atkSpeed, 2.5f, 1, 1);
		unitAI.MyHp.onDeathAction += Dead;
		
		//공속 증가에 따른 애니메이션 속도 조절
		player.StatDic[StatType.AtkSpeed].effect.Subscribe(value =>
		{
			unitAI.Animtor.SetFloat(AnimationHash.Attack_SPEED_ANIM_HASH, 1 + ((float)value * 0.01f));
		}).AddTo(this);
        
		Observable.Interval(TimeSpan.FromSeconds(1f)).Subscribe(_ =>
		{
			var value = player.StatDic[StatType.Heal].effect.Value;
			unitAI.MyHp.Heal(value);
		}).AddTo(this);

		StageManager.Instance.onInitStage += () => ResetHP().Forget();
        
	}

	protected override void OnAttack()
	{
		if (unitAI.MyHp.IsDead || unitAI.Target == null)
			return;
		
		float criRate = Random.Range(0, 1f);
		bool isCritical = criRate >= player.StatDic[StatType.CriRate].effect.Value * 0.01f;

		double damage = player.GetTotalDamage();
		var finalDamage = isCritical ? damage * player.StatDic[StatType.CriDamge].effect.Value : damage;
        
		unitAI.Target.Hit(finalDamage,isCritical);
	}
	public override void Attack()
	{
		isAttack = true;
		unitAI.Animtor.SetTrigger(AnimationHash.Attack_ANIM_HASH);
	}

	protected override void EndAttack()
	{
		isAttack = false;
		SetCoolTime();
	}

	public override void UpdateCoolTime(float deltaTime)
	{
		base.UpdateCoolTime(deltaTime);
		player.SkillSystem.UpdateSkillCoolTime(deltaTime);

		foreach (var pet in player.EquippedPet)
		{
			if (pet != null)
			{
				pet.UpdateCoolTime(deltaTime);
			}
		}
	}
	protected override void Dead()
	{
		StageManager.Instance.OnDeadSubject.OnNext(default);
	}

	private async UniTaskVoid ResetHP()
	{
		await UniTask.Yield();
		Player.Instance.PlayerUnit.MyHp.ResetHp();
	}
	protected override void SetCoolTime()
	{
		Stat.AtkCoolTime = 1 / (Stat.AtkSpeed + (float)player.StatDic[StatType.AtkSpeed].effect.Value);
	}

	public void EquipPet(int idx, RuntimeAnimatorController AC)
	{
		petAnimators[idx].enabled = true;
		petSpriteRenderers[idx].enabled = true;
		petAnimators[idx].runtimeAnimatorController = AC;
	}

	public void UnEquipPet(int idx)
	{
		petAnimators[idx].enabled = false;
		petSpriteRenderers[idx].enabled = false;
	}
}