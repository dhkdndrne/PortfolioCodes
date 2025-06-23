using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Special_ShapeChecker : ShapeChecker
{
	private List<Block> list2 = new List<Block>();
	private List<Block> list3 = new List<Block>();
	private List<Block> list4 = new List<Block>();

	private void CheckAndAddBlocks(Block block, HexWay way1, HexWay way2, List<Block> tempList)
	{
		tempList.Clear();
		ShapeCheckUtil.SearchBlockLine(block, way1, way2, tempList);
		if (tempList.Count >= specialBlockData.Condition)
		{
			list.AddRange(tempList);
		}
	}
	public override bool CheckShape(Block block, HashSet<Block> popList = null, List<ReservedSBlockData> itemList = null)
	{
		list.Clear();

		CheckAndAddBlocks(block, HexWay.RightDown, HexWay.LeftUp, list2);
		CheckAndAddBlocks(block, HexWay.LeftDown, HexWay.RightUp, list3);
		CheckAndAddBlocks(block, HexWay.Up, HexWay.Down, list4);
		
		if (list.Count > 0)
		{
			popList?.AddRange(list);
			itemList?.Add(new ReservedSBlockData(specialBlockData,ColorLayer.None,block.Hex, new List<Block>(list)));
		}
		
		return list.Count > 0;
	}
}