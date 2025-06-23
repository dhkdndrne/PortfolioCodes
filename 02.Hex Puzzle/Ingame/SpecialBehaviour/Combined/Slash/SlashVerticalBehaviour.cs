using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SlashVerticalBehaviour : MonoBehaviour, ISpecialBlockBehaviour
{
	private HashSet<Block> hashSet;
	public void Execute(Hex hex,SpecialBlockType blockType, HashSet<Block> withinRangeList)
	{
		var centerHex = hex;

		ShapeCheckUtil.Search(centerHex, HexWay.Up, withinRangeList);
		ShapeCheckUtil.Search(centerHex, HexWay.Down, withinRangeList);
		
		if (blockType is SpecialBlockType.LSlash_Vertical)
		{
			ShapeCheckUtil.Search(centerHex, HexWay.LeftDown, withinRangeList);
			ShapeCheckUtil.Search(centerHex, HexWay.RightUp, withinRangeList);
		}
		else
		{
			ShapeCheckUtil.Search(centerHex, HexWay.RightDown, withinRangeList);
			ShapeCheckUtil.Search(centerHex, HexWay.LeftUp, withinRangeList);
		}
		hashSet = withinRangeList;
	}

	public async UniTask Anim(Block block)
	{
		await GameManager.Instance.Board.ShowTargetHighlightAll(hashSet);
	}
}