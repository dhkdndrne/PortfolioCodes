using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Vertical_ShapeChecker : ShapeChecker
{
	public override bool CheckShape(Block block, HashSet<Block> popList = null, List<ReservedSBlockData> itemList = null)
	{
		list.Clear();
		ShapeCheckUtil.SearchBlockLine(block, HexWay.Up, HexWay.Down, list);

		bool isSatisfyItemCondition = list.Count >= specialBlockData.Condition;
		bool isSatisfyMinCondition = list.Count >= minCondition;

		if (isSatisfyItemCondition || isSatisfyMinCondition)
		{
			popList?.AddRange(list);
		}

		if (isSatisfyItemCondition)
		{
			itemList?.Add(new ReservedSBlockData(specialBlockData,block.ColorLayer,block.Hex, new List<Block>(list)));
		}

		return isSatisfyMinCondition;
	}
}