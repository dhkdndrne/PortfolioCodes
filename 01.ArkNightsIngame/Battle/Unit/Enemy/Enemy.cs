using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class Enemy : Unit
{
	/*
	 *	[적의 상태]
	 *  대기,이동,공격
	 *	공격은 범위에 들어오면 언제든지
	 *	블록당했으면 제자리에 서기
	 *  블록당하지 않았으면 공격 후 할일하기 (이동 or 대기)
	 *
	 */

	[SerializeField] private int enemyID;

	private EnemySpriteHandler spriteHandler;
	[SerializeField] private SliderHandler sliderHandler;
	public EnemyAttribute Attribute => (EnemyAttribute)attribute;
	
	private void Awake()
	{
		spriteHandler = GetComponent<EnemySpriteHandler>();
	}

	public override void Init()
	{
		var data = DataManager.Instance.EnemyData.GetEnemyData(enemyID);
		attribute = new EnemyAttribute(data.Hp, data.AttackCoolTime, data.AttackPower, data.Defense, data.MagicDefense, data.MoveSpeed);
	}

	public override bool IsDead()
	{
		return attribute.Hp.Value <= 0;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		if (attribute as EnemyAttribute != null)
		{
			attribute.ResetHp();
			sliderHandler.ChainEvent(attribute.HpRatio, 1);
		}
	}
	
	protected override void OnUnitDead()
	{
		sliderHandler.UnChainEvent(attribute.Hp);
	}
	public override void Hit(Unit attacker)
	{
		attribute.Hp.Value -= CombatFormulaUtil.GetFinalMeleeDamage(attacker, this);
		spriteHandler.HitSpriteEffect().Forget();
		if (attribute.Hp.Value <= 0)
		{
			InvokeOnDeath();
		}
	}
	public void Attack(Unit target)
	{
		target.Hit(this);
	}
}