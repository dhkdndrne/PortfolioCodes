using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShapeCheckUtil
{
	/// <summary>
	/// 직선 체크
	/// </summary>
	public static void SearchBlockLine(Block block, HexWay way1, HexWay way2, List<Block> list)
	{
		var board = GameManager.Instance.Board;

		// 첫 블록 넣어줌
		list.Add(block);
		SearchBlockByWay(board, block, block.Hex, way1, list);
		SearchBlockByWay(board, block, block.Hex, way2, list);
	}

	/// <summary>
	/// way 방향으로 같은 블록들 리스트에 넣어줌
	/// </summary>
	private static void SearchBlockByWay(Board board, Block block, Hex beginHex, HexWay way, List<Block> list)
	{
		Hex curHex = beginHex;
		while (true)
		{
			curHex = Hex.GetHexByWay(curHex, way);

			if (board.IsIndexOutOfRange(curHex)) break;
			if (!board.IsValidIndex(curHex)) break;

			var nextBlock = board.GetBlock(curHex);
			if (nextBlock == null) break;
			if (!nextBlock.CanMatchWith(block)) break;
			
			list.Add(nextBlock);
		}
	}

	public static void Search(Hex hex, HexWay way, HashSet<Block> withinRangeList)
	{
		var board = GameManager.Instance.Board;
		var nHex = hex;
		withinRangeList.Add(board.GetBlock(nHex));
		
		while (true)
		{
			nHex = Hex.GetHexByWay(nHex, way);

			if (board.IsIndexOutOfRange(nHex)) break;
			if (!board.IsValidIndex(nHex)) break;

			var nb = board.GetBlock(nHex);
			if (nb == null) continue;
			if (nb.BlockData is SpecialBlockData specialBlockData && specialBlockData.SBlockType == SpecialBlockType.Super) continue;
			
			withinRangeList.Add(nb);
		}
	}
}