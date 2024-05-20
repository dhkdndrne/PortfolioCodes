using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Equipment : ItemBase
{
	public double EquippedEffect { get; private set; }
	public EquipmentType Equipment_Type { get; private set; }

	public void Init(int id,bool isLock, string itemName, EquipmentType equipmentType, ItemRankType rank, int level, int maxLevel, int piece, int maxPiece, OwnEffect ownEffect, double equippedEffect, string description)
	{
		base.Init(id,isLock, itemName,rank, level, maxLevel, piece, maxPiece, ownEffect,description);
        
		Equipment_Type = equipmentType;
		EquippedEffect = equippedEffect;
	}
}