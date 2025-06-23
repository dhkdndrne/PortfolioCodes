using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BombVerticalBehaviour : MonoBehaviour, ISpecialBlockBehaviour
{
	private List<Block> list = new();
	private HashSet<Block> hashSet;
	
	public void Execute(Hex hex,SpecialBlockType blockType, HashSet<Block> withinRangeList)
	{
		list.Clear();
		var centerHex = hex;
		var board = GameManager.Instance.Board;

		list.Add(board.GetBlock(centerHex));
		var leftBlock = GetValidBlock(board, centerHex, HexWay.LeftDown, HexWay.LeftUp);
		if (leftBlock != null)
		{
			list.Add(leftBlock);
		}

		var rightBlock = GetValidBlock(board, centerHex, HexWay.RightDown, HexWay.RightUp);
		if (rightBlock != null)
		{
			list.Add(rightBlock);
		}

		foreach (var b in list)
		{
			withinRangeList.Add(b);
			ShapeCheckUtil.Search(b.Hex, HexWay.Up, withinRangeList);
			ShapeCheckUtil.Search(b.Hex, HexWay.Down, withinRangeList);
		}

		hashSet = withinRangeList;
	}
	public async UniTask Anim(Block block)
	{
		await GameManager.Instance.Board.ShowTargetHighlightAll(hashSet);
	}

	private Block GetValidBlock(Board board, Hex centerHex, HexWay primaryWay, HexWay fallbackWay)
	{
		var hex = Hex.GetHexByWay(centerHex, primaryWay);

		bool isValid = Check(board, hex);
        
		if (!isValid)
		{
			hex = Hex.GetHexByWay(centerHex, fallbackWay);
			isValid = Check(board, hex);
		}

		return isValid ? board.GetBlock(hex) : null;
	}

	private bool Check(Board board, Hex hex)
	{
		return !board.IsIndexOutOfRange(hex) && board.IsValidIndex(hex);
	}
}