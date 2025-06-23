using System;
using System.Collections.Generic;
using UnityEngine;
using Bam.Extensions;

public class EnemyController : MonoBehaviour
{
    #region AnimationHash

	private readonly int ATTACK_ANIM_HASH = Animator.StringToHash("Attack");
	private readonly int Move_ANIM_HASH = Animator.StringToHash("isMove");
	private readonly int ATTACK_SPEED_ANIM_HASH = Animator.StringToHash("AttackSpeed");
	private readonly int MOVE_SPEED_ANIM_HASH = Animator.StringToHash("MoveSpeed");

    #endregion

	private Enemy enemyUnit;
	private Unit target;
	private Transform objBody;

	private StateMachine stateMachine;
	private Animator animator;

	private float atkCoolTime;
	private bool isAttacking;
	private bool isBlocked;
	private int wayPointIndex;

	private List<WayPoint> wayPoints;
	//private List<Tile> curTile; // 0 이전타일 , 1 다음 타일

	private Tile currentTile;
	private Tile beforeTile;
	private Tile targetTile;
	public event Action OnArrival; //적이 도착했을때 동작할 이벤트
	public Enemy EnemyUnit => enemyUnit;
	private void Awake()
	{
		stateMachine = new StateMachine();
		//curTile = new List<Tile>();
		animator = GetComponentInChildren<Animator>();
		objBody = animator.transform;

		enemyUnit = GetComponent<Enemy>();

		Init();
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
	
	private void Init()
	{
		enemyUnit.Init();
		InitState();

		enemyUnit.OnDeath += () =>
		{
			wayPointIndex = 0;
			currentTile?.Enemies.Remove(enemyUnit);
			beforeTile?.Enemies.Remove(enemyUnit);
			targetTile?.Enemies.Remove(enemyUnit);
			
			if (target != null)
			{
				var op = target as Operator;
				if (op != null)
				{
					op.Block++;
				}
			}
			GameManager.Instance.Stage.AddKillCount();
		};

		var enemyEventHandler = GetComponentInChildren<EnemyEventHandler>();
		enemyEventHandler.OnAttackAction += Attack;
		enemyEventHandler.OnEndAction += End;
	}

	private void InitState()
	{
		stateMachine.AddState((int)Enemy_State.Idle, new State().OnEnter(() =>
		{
			target = null;
		}));

		// 다음 타일에 등록 했는지
		bool isRegistNextTile = false;
		bool isWaiting = false;
		bool check = false;
		float duration = 0f;
		
		stateMachine.AddState((int)Enemy_State.Move, new State().OnEnter(() =>
		{
			currentTile = GameManager.Instance.TileManager.GetTile(wayPoints[0].Position);
			currentTile.Enemies.Add(enemyUnit);

			targetTile = currentTile;

			animator.SetBool(Move_ANIM_HASH, true);
		}).OnStay(() =>
		{
			if (wayPointIndex < wayPoints.Count)
			{
				// 대기
				if (isWaiting)
				{
					duration -= CustomTime.deltaTime;
					if (duration <= 0)
					{
						isWaiting = false;
						wayPointIndex++;
						isRegistNextTile = false;
					}
					return;
				}

				var targetPosition = wayPoints[wayPointIndex].Position;
				var dir = (targetPosition - transform.position).normalized;

				//적 스프라이트 flip
				int face = 1;
				if (dir.x > 0.5) face = -1;
				else if (dir.x < -0.5) face = 1;

				objBody.transform.localScale = new Vector3(0.4f * face, 0.4f, 0.4f);
				transform.position = Vector3.MoveTowards(transform.position, targetPosition, enemyUnit.Attribute.MoveSpeed * CustomTime.deltaTime);

				var targetDistance = Extensions.Distance(transform.position, currentTile.transform.position);
				if (targetDistance <= 0.1f && !check)
				{
					beforeTile = currentTile;
					isRegistNextTile = false;
					if (targetTile == currentTile)
					{
						//만약 대기해야하는 타일이면 대기활성화
						if (wayPoints[wayPointIndex].Type == WayPointType.Stay)
						{
							duration = wayPoints[wayPointIndex].WaitTime;
							isWaiting = true;
						}
						else
						{
							wayPointIndex++;
							check = true;
						}

						if (wayPointIndex < wayPoints.Count)
							targetTile = GameManager.Instance.TileManager.GetTile(wayPoints[wayPointIndex].Position);
					}
				}

				if (beforeTile != null)
				{
					var dist = Extensions.Distance(transform.position, beforeTile.transform.position);
					if (dist >= 0.8f)
					{
						check = false;

						beforeTile.Enemies.Remove(enemyUnit);
						beforeTile = null;
					}
				}

				// 이동경로에 잇는 블록
				// 이동경로에 있는 타일 처리
				int pathX = Mathf.RoundToInt(transform.position.x);
				int pathZ = Mathf.RoundToInt(transform.position.z);
				var newTile = GameManager.Instance.TileManager.Tiles[pathZ, pathX];
				
				if (currentTile != newTile)
				{
					// 앞으로 이동할 타일과의 거리 계산
					var nextTileDistance = Extensions.Distance(transform.position, newTile.transform.position);
					int currentX = Mathf.RoundToInt(transform.position.x);
					int currentZ = Mathf.RoundToInt(transform.position.z);
					int nextX = Mathf.RoundToInt(newTile.transform.position.x);
					int nextZ = Mathf.RoundToInt(newTile.transform.position.z);

					int diffX = Mathf.Abs(currentX - nextX);
					int diffZ = Mathf.Abs(currentZ - nextZ);

					bool isDiagonal = diffX == 1 && diffZ == 1;
					float value = isDiagonal ? 1.4f : 0.6f;

					if (nextTileDistance <= value && !isRegistNextTile)
					{
						isRegistNextTile = true;

						if (isDiagonal)
						{
							// 이동 방향에 따라 올바른 타일 좌표 보정
							currentX = (dir.x < 0) ? Mathf.FloorToInt(transform.position.x) : Mathf.CeilToInt(transform.position.x);
							currentZ = (dir.z < 0) ? Mathf.FloorToInt(transform.position.z) : Mathf.CeilToInt(transform.position.z);
						}

						var nextTile = GameManager.Instance.TileManager.Tiles[currentZ, currentX];
						nextTile.Enemies.Add(enemyUnit);
						currentTile = newTile;
					}
				}
			}
			else
			{
				OnArrival?.Invoke();
				GameManager.Instance.Stage.ReduceLife();
				wayPointIndex = 0;
				gameObject.SetActive(false);
			}
		}).AddCondition((int)Enemy_State.Attack, () =>
		{
			Operator op = currentTile.UnitOnTile;
			
			if (op != null && op.Block > 0)
			{
				op.Block--;
				target = op;
				return true;
			}

			return false;
		}).OnEnd(() =>
		{
			animator.SetBool(Move_ANIM_HASH, false);
		}));

		stateMachine.AddState((int)Enemy_State.Attack, new State().OnEnter(() =>
		{

		}).OnStay(() =>
		{
			if (atkCoolTime > 0 || isAttacking) return;
			isAttacking = true;
			animator.SetTrigger("Attack");

		}).AddCondition((int)Enemy_State.Idle, () =>
		{
			return target == null || target.IsDead();

		}));
	}

	private void Update()
	{
		if (enemyUnit.IsDead())
			return;
		
		UpdateCoolTime();
		stateMachine.Update();
	}
	
	private void Attack()
	{
		enemyUnit.Attack(target);
	}

	private void End()
	{
		isAttacking = false;
		atkCoolTime = enemyUnit.Attribute.AttackSpeed;
	}
	private void UpdateCoolTime()
	{
		if (atkCoolTime > 0)
		{
			atkCoolTime -= CustomTime.deltaTime;

			if (atkCoolTime <= 0)
				atkCoolTime = 0;
		}
	}
	public void StartActive(List<WayPoint> wayPoints)
	{
		this.wayPoints = wayPoints;
		stateMachine.ChangeState((int)Enemy_State.Move);
	}
}