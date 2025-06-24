using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(OperatorController))]
public abstract class Operator : Unit
{
	[SerializeField] private OperatorID operatorID;
	[SerializeField] protected List<Tile> tilesInAttackRange; //공격범위 안 타일
	[SerializeField] protected SubProfessionData subProfessionData;

	private RangeData rangeData;
	protected OperatorController controller;
	protected Tile onTile; //올라가 있는 타일
	private GridType[,] attackRange;
	protected List<TalentHandler> talents = new List<TalentHandler>();
	public Tile OnTile => onTile;
	public Action onDeploy;
	public Action onRetreat;
	public event Action onHit;
	private int block;
	private bool isDead;
	private OperatorDirection dir;
	public OperatorController Controller => controller;
	public OperatorAttribute Attribute => (OperatorAttribute)attribute;
	public Operator_AtkType AtkType { get; private set; }
	public Operator_Class OperatorClass { get; private set; }
	public IReadOnlyList<TalentHandler> Talents => talents;
	public OperatorID OperatorID => operatorID;
	public SubProfessionData SubProfessionData => subProfessionData;

	public GridType[,] GetOriginAttackRangeGrid() => rangeData.GetGridArray();
	public GridType[,] GetCurAttackRangeGrid() => attackRange;
	public ObservableValue<int> Cost => Attribute.Cost;
	public IReadOnlyList<Tile> TilesInAttackRange => tilesInAttackRange;
	public int MaxBlock => Attribute.Block;
	public int Block
	{
		get => block;
		set
		{
			block = value;

			var max = Attribute.Block;
			if (block >= max)
				block = max;
		}
	}

	public OperatorDirection Direction => dir;
	private void Awake()
	{
		tilesInAttackRange = new List<Tile>();
		talents = GetComponentsInChildren<TalentHandler>().ToList();
	}
	
	public void SetController(OperatorController controller) => this.controller = controller;

	public override void Init()
	{
		var data = DataManager.Instance.OperatorData.GetOperatorData((int)operatorID);

		unitName = data.Name;
		AtkType = data.OperatorAtkType;
		OperatorClass = data.OperatorClass;
		rangeData = DataManager.Instance.RangeDic[data.RangeID];

		//todo 레벨받아오기
		var levelData = data.GetLevelData(1);

		attribute = new OperatorAttribute(levelData.MaxHp, levelData.AttackCoolTime,
			levelData.Atk_Power, levelData.Defense, levelData.Magic_Resistance, levelData.RedeployTime, levelData.Cost, levelData.Block);

		block = Attribute.Block;


		

		onDeploy += () =>
		{
			isDead = false;
			SetTilesInAttackRange();
			Attribute.ResetHp();
		};

		onRetreat += () =>
		{
			isDead = true;

			//퇴각시 코스트 반 반환
			int halfCost = Cost.Value / 2;
			GameManager.Instance.Stage.ChangeCostValue(halfCost);

			InvokeOnDeath();
		};

		//todo 나중에 범위데이터 어드레서블로 받아오기
		attackRange = rangeData.GetGridArray();
		
		//재능 설정
		foreach (var t in talents)
		{
			t.Initialize(this);
		}
	}

	public virtual void Tick()
	{
		float dt = Time.deltaTime;
		foreach (var kvp in activeBuffsDict.ToList())
		{
			var buffList = kvp.Value;
			for (int i = buffList.Count - 1; i >= 0; i--)
			{
				var buff = buffList[i];

				// 그룹 버프는 BuffGroup 쪽에서 처리하니 건너뛰기
				if (buff.IsGroup) continue;
				if (buff.IsInfinite) continue;

				buff.Update(dt);
				if (buff.IsExpired)
					RemoveBuff(buff);
			}
		}
	}
	
	public void SetTile(Tile tile)
	{
		onTile = tile;
	}

	public override bool IsDead()
	{
		return isDead;
	}

	public override void Hit(Unit attacker)
	{
		float incomingDamage = attacker.GetFinalDamage();
		float remainingDamage = incomingDamage;

		if (activeBuffsDict.TryGetValue(typeof(Buff_Shield), out List<Buff> shieldBuffs))
		{
			foreach (Buff_Shield buff in shieldBuffs)
			{
				remainingDamage = buff.AbsorbDamage(remainingDamage);

				if (remainingDamage <= 0)
					break;
			}
		}

		if (remainingDamage > 0)
		{
			attribute.Hp.Value -= remainingDamage;
		}

		onHit?.Invoke();
		if (attribute.Hp.Value <= 0)
		{
			isDead = true;
			InvokeOnDeath();
		}
	}

	public virtual void GetTargets(HashSet<Unit> targetList)
	{
		targetList.Clear();

		//타일 위 지나가는 적들 등록
		foreach (var tile in tilesInAttackRange)
		{
			targetList.AddRange(tile.Enemies);
		}
	}

	/// <summary>
	/// 공격범위 안의 타일 설정
	/// 기본적으로 유닛의 공격범위로 설정
	/// 스킬 사용시 범위 바뀌는 스킬이면 매개변수로 범위 데이터 전달
	/// </summary>
	private void SetTilesInAttackRange()
	{
		tilesInAttackRange.Clear();
		tilesInAttackRange = AttackRangeHandler.GetAttackableTiles(onTile, attackRange);
	}

	public void SetAttackDir(OperatorDirection dir,GridType[,] newRange = null)
	{
		this.dir = dir;
		var range = newRange == null ? GetOriginAttackRangeGrid() : newRange;
		attackRange = Bam.Extensions.Extensions.RotateArray(range, ((int)dir - 1) * 90);
	}

	
	public abstract void Attack(HashSet<Unit> targetList);
	public abstract Skill GetSkill();

	/// <summary>
	/// 스킬 시전 시, 스킬 데이터에 있는 rangeId를 이용해 공격 범위를 변경합니다.
	/// </summary>
	public void ChangeAttackRange(int rangeId)
	{
		if (DataManager.Instance.RangeDic.TryGetValue(rangeId, out RangeData newRangeData))
		{
			GridType[,] newRange = newRangeData.GetGridArray();
			SetAttackDir(dir,newRange);
			SetTilesInAttackRange();
		}
	}

	/// <summary>
	/// 스킬 시전이 종료되면 기본 공격 범위로 복원합니다.
	/// </summary>
	public void RevertAttackRange()
	{
		SetAttackDir(dir);
		SetTilesInAttackRange();
	}

	protected override void OnUnitDead()
	{
		bool isFirstDead = true;
		int firstCost = Cost.Value;
		
		onTile.RemoveUnit();
		SetTile(null);
		block = ((OperatorAttribute)attribute).Block;
		attackRange = rangeData.GetGridArray();

		Cost.Value = isFirstDead ? (int)(firstCost * 1.5f) : firstCost * 2;

		if (Cost.Value >= 99)
			Cost.Value = 99;

		isFirstDead = false;
		GameManager.Instance.RemoveOperator(this);
	}
}