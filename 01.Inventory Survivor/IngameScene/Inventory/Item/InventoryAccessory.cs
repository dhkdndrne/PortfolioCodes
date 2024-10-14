using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class InventoryAccessory : InventoryItem
{
	public void ApplyItemAbility(bool isRemove)
	{
		var data = itemSo as UnattackableItemSo;
		foreach (var val in data.GetAbility())
		{
			PlayerData.Instance.ChangeAbilityValue(val.ability, isRemove ? -val.value : val.value);
		}
	}
	protected override void EquipItem()
	{
		ApplyItemAbility(false);
	}
}