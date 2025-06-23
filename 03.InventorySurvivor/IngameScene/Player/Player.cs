using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Bam.Extensions;
using Unity.Mathematics;

public class Player : Creature
{
	private PlayerWeaponManager weaponManager;
	
	private void Start()
	{
		weaponManager = GetComponent<PlayerWeaponManager>();
		
		GameManager.Instance.Step.Where(step => step is GameStep.Playing).Subscribe(_ =>
		{
			Observable.EveryUpdate()
				.TakeUntil(GameManager.Instance.Step.Where(s => s != GameStep.Playing).DistinctUntilChanged())
				.Subscribe(__ =>
				{
					weaponManager.UpdateCoolTime(Time.deltaTime);
				}).AddTo(this);
		}).AddTo(this);
	}
	
	public override void Hit(DamageType damageType,float damage)
	{
		PlayerData.Instance.GetAbilityProperty(AbilityType.Hp).Value -= (int)damage;
	}
	
	protected override void Dead()
	{
		
	}
}
