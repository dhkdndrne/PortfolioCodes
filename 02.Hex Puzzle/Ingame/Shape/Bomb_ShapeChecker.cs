using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bomb_ShapeChecker : ShapeChecker
{
	private List<Block> upDownList = new List<Block>();
	private List<Block> left_D_Rigt_U_List = new List<Block>();
	private List<Block> left_U_Right_D_List = new List<Block>();
	public override bool CheckShape(Block block, HashSet<Block> popList = null, List<ReservedSBlockData> itemList = null)
	{
		list.Clear();
		upDownList.Clear();
		left_U_Right_D_List.Clear();
		left_D_Rigt_U_List.Clear();
		
		ShapeCheckUtil.SearchBlockLine(block, HexWay.Up, HexWay.Down, upDownList);
		ShapeCheckUtil.SearchBlockLine(block, HexWay.RightDown, HexWay.LeftUp, left_U_Right_D_List);
		ShapeCheckUtil.SearchBlockLine(block, HexWay.LeftDown, HexWay.RightUp, left_D_Rigt_U_List);

		int cnt = CheckCondition(upDownList);
		cnt += CheckCondition(left_U_Right_D_List);
		cnt += CheckCondition(left_D_Rigt_U_List);
		
		if (cnt >= 2)
		{
			popList?.AddRange(list);
			itemList?.Add(new ReservedSBlockData(specialBlockData,block.ColorLayer,block.Hex,new List<Block>(list)));
		}
        
		return cnt >= 2;
	}

	private int CheckCondition(List<Block> l)
	{
		if (l.Count >= minCondition)
		{
			list.AddRange(l);
			return 1;
		}

		return 0;
	}
}
