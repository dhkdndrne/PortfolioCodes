using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boomerang_ShapeChecker : ShapeChecker
{
	public override bool CheckShape(Block block, HashSet<Block> popList = null, List<ReservedSBlockData> itemList = null)
	{
		var board = GameManager.Instance.Board;
		for (int i = 0; i < (int)HexWay.Length; i++)
		{
			if (CheckMatch(board, block, i, true) || CheckMatch(board, block, i, false))
			{
				list.Add(block);
				popList?.AddRange(list);
				itemList?.Add(new ReservedSBlockData(specialBlockData,block.ColorLayer, block.Hex, new List<Block>(list)));
				return true;
			}
		}

		return false;
	}
	private bool CheckMatch(Board board, Block block, int start, bool isCentral)
	{
		list.Clear();
		bool isSameColor = true;

		for (int i = start, cnt = 3; cnt > 0; cnt--)
		{
			var hex = Hex.GetHexByWay(block.Hex, (HexWay)i);

			// 외곽 블록일 경우
			if (!isCentral && cnt == 1)
			{
				int index = i - 1 < 0 ? (int)HexWay.Length - 1 : i - 1;
				//hex = Hex.GetHexByWay(list[list.Count - 2].Hex, (HexWay)(index));
			}

			if (board.IsIndexOutOfRange(hex)) return false;
			if (!board.IsValidIndex(hex)) return false;

			var nb = board.GetBlock(hex);
			if (nb == null) return false;

			isSameColor &= block.ColorLayer == nb.ColorLayer;
			if (!isSameColor) return false;

			list.Add(nb);
			
			i++;
			if (i == (int)HexWay.Length)
				i = 0;
		}

		return true;
	}
}