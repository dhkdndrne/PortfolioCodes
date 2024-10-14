using System.Collections;
using System.Collections.Generic;
using Bam.Singleton;
using UnityEngine;

public class PopUpManager : Singleton<PopUpManager>
{
	[SerializeField] private ItemInfoUI itemInfoUI;
	[SerializeField] private SynergyPanel synergyPanel;

	public void ShowShopItemInfo(ItemSo itemSo)
	{
		float size = itemSo.ItemType is ItemType.Weapon ? 620f : 370f;
		itemInfoUI.SetPosition(size);
		itemInfoUI.ShowShopItemInfo(itemSo);
	}
	public void ShowInventoryItemInfo(InventoryItem item)
	{
		float size = item.ItemSo.ItemType is ItemType.Weapon ? 753f : 544f;
		itemInfoUI.SetPosition(size);
		itemInfoUI.ShowInventoryItemInfo(item);

		if (item.ItemSo.ItemType is ItemType.Weapon)
		{
			synergyPanel.gameObject.SetActive(true);
			synergyPanel.UpdateUI(item.ItemSo as AttackItemSo);
		}
			
	}

	public void DisableItemInfo()
	{
		itemInfoUI.gameObject.SetActive(false);
		synergyPanel.gameObject.SetActive(false);
	}
}