using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Slash_ShapeChecker : ShapeChecker
{
	[Header("↖↘ 방향으로 터지는지")]
	[SerializeField] private bool isRight;

	public override bool CheckShape(Block block, HashSet<Block> popList = null, List<ReservedSBlockData> itemList = null)
	{
		list.Clear();
		if (isRight)
		{
			ShapeCheckUtil.SearchBlockLine(block, HexWay.RightDown, HexWay.LeftUp, list);
		}
		else
		{
			ShapeCheckUtil.SearchBlockLine(block, HexWay.LeftDown, HexWay.RightUp, list);
		}


		if (list.Count >= minCondition)
			popList?.AddRange(list);


		if (itemList != null)
		{
			bool isSatisfyItemCond = list.Count >= specialBlockData.Condition;

			if (isSatisfyItemCond)
			{
				itemList.Add(new ReservedSBlockData(specialBlockData,block.ColorLayer,block.Hex, new List<Block>(list)));
			}

		}
		return list.Count >= minCondition;
	}
}