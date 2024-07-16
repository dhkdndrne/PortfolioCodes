using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryWeapon : InventoryItem
{
	[SerializeField] private float[] bonusStat;
	private new AttackItemSo ItemSo;
	
	protected override void Awake()
	{
		base.Awake();
		bonusStat = new float[(int)AbilityType.Length];
	}

	public float GetBounusStat(AbilityType abilityType) => bonusStat[(int)abilityType];
	public override void Init(ItemSo itemSo)
	{
		base.Init(itemSo);
		ItemSo = itemSo as AttackItemSo;
	}
	protected override void EquipItem()
	{
		SynergyManager.Instance.AddSynergy(itemSo as AttackItemSo);
	}

	public void SetBonusStat(BuffSkill.BuffToken buffToken)
	{
		bonusStat[(int)buffToken.abilityType] = buffToken.value;
	}
	public void RemoveStat(BuffSkill.BuffToken buffToken)
	{
		bonusStat[(int)buffToken.abilityType] -= buffToken.value;
	}
}