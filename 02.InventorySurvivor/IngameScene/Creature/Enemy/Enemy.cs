using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Creature
{
	[SerializeField] private CreatureStatSo stat;
	
	private EnemyAI enemyAI;

	#region 스탯

	private bool isAttacking;
	
	private float attackCoolTime;
	private float moveSpeed;
	
	public float MoveSpeed => moveSpeed;
	public float Hp => hp;
    #endregion

    
	public void Init(EnemyAI enemyAI)
	{
		this.enemyAI = enemyAI;
		SetStat();
	}

	private void SetStat()
	{
		hp = stat.Hp;
		maxHp = hp;
		
		attackCoolTime = 0;
		isAttacking = false;
		moveSpeed = stat.MoveSpeed;
	}
	
	public override void Hit(DamageType damageType,float damage)
	{
		
	}
	protected override void Dead()
	{
		
	}

	public void UpdateCoolTime(float delta)
	{
		attackCoolTime -= delta;

		if (attackCoolTime <= 0)
			attackCoolTime = 0;
	}

	public bool CanAttack => !isAttacking && attackCoolTime == 0;
	
	public void Attack()
	{
		enemyAI.Animator.SetTrigger(Define.Attack_ANIM_HASH);
		isAttacking = true;
		
	}
	
	protected void OnAttack()
	{
		enemyAI.Target.Hit(stat.DamageType,stat.AtkPower);
	}  
	protected void End()
	{
		attackCoolTime = stat.AtkCoolTime;
		isAttacking = false;
	} 
}
