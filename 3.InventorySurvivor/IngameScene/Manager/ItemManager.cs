using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemManager : Manager<ItemManager>
{
	[SerializeField] private ItemListToken[] itemLists;

	private Dictionary<ItemType, Dictionary<ItemRarity, List<ItemSo>>> itemDic;
	
	public Dictionary<ItemType,List<InventoryItem>> equipedItemList;
	
	public override void OnStartManager()
	{
		Init();
		InitWeaponData().Forget();
	}
	private void Init()
	{
		itemDic = new();

		foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)))
		{
			itemDic.Add(itemType, new Dictionary<ItemRarity, List<ItemSo>>());

			foreach (ItemRarity rarity in Enum.GetValues(typeof(ItemRarity)))
			{
				itemDic[itemType].Add(rarity, new List<ItemSo>());
			}
		}

		foreach (var itemList in itemLists)
		{
			var itemListSo = itemList.listSo.List;

			foreach (var so in itemListSo)
			{
				itemDic[itemList.itemType][so.Rarity].Add(so);
			}
		}
		
		equipedItemList = new Dictionary<ItemType, List<InventoryItem>>();
		foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)))
		{
			equipedItemList.Add(itemType,new List<InventoryItem>());
		}
	}
	private async UniTaskVoid InitWeaponData()
	{
		var data = await DataManager.Instance.GetWeaponData();

		for (int i = 1; i < data.Length; i++)
		{
			string[] column = data[i].Split(',');
			int id = int.Parse(column[0]);
			string name = column[1];
			ItemRarity rarity = (ItemRarity)Enum.Parse(typeof(ItemRarity), column[2]);
			DamageType damageType = (DamageType)Enum.Parse(typeof(DamageType), column[3]);
			
			int price = int.Parse(column[4]);
			float applyAbilityVal = float.Parse(column[5]);
			float damage = float.Parse(column[6]);
			float criChace = float.Parse(column[7]);
			float criPower = float.Parse(column[8]);
			float range = float.Parse(column[9]);
			float atkSpeed = float.Parse(column[10]);
			var synergyIDs = Array.ConvertAll(column[11].Split('/'), int.Parse);

			foreach (var val in itemDic[ItemType.Weapon][rarity])
			{
				if (val.ID == id)
				{
					(val as AttackItemSo).SetData(name,rarity,damageType,price,applyAbilityVal,damage,criChace,criPower,range,atkSpeed,synergyIDs);
					break;
				}
			}
		}
	}
	
	public ItemSo GetRandomItem()
	{
		ItemType randomType = GetRandomEnumValue<ItemType>();
		ItemRarity randomRarity = GetRandomEnumValue<ItemRarity>();

		//var itemList = itemDic[randomType][randomRarity];	//나중에~
		
		var itemList = itemDic[ItemType.Weapon][randomRarity];
		return itemList[Random.Range(0, itemList.Count)];
		
		//var itemList = itemDic[ItemType.Accessory][0];
		//return itemDic[ItemType.Accessory][ItemRarity.Common][0];
	}
	
	public void EquipItem(InventoryItem item)
	{
		var itemType = item.ItemSo.ItemType;
		equipedItemList[itemType].Add(item);
	}
	
	public void UnEquipItem(InventoryItem item)
	{
		var itemType = item.ItemSo.ItemType;
		equipedItemList[itemType].Remove(item);
	}
	
	private T GetRandomEnumValue<T>()
	{
		Array values = Enum.GetValues(typeof(T));
		return (T)values.GetValue(Random.Range(0, values.Length));
	}

	public void CheckNeighborBuffSkill(InventoryItem item)
	{
		if (item.HasBuffSkill())
		{
			var skill = item.Skill as BuffSkill;
			skill.AddBuffValue(item.NeighborItem);

			var weapon = item as InventoryWeapon;
			
			foreach (var buff in skill.GetBuff())
				weapon.SetBonusStat(buff);
		}
	}
	
	[Serializable]
	private class ItemListToken
	{
		public ItemType itemType;
		public ItemSoList listSo;
	}
}