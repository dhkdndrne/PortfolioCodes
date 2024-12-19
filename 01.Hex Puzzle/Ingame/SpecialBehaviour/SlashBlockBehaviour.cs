using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class SlashBlockBehaviour : MonoBehaviour, ISpecialBlockBehaviour
{
	[SerializeField] private bool isLeftDown;
	[SerializeField] private GameObject leftDown;
	[SerializeField] private GameObject leftUp;
	[SerializeField] private GameObject rightDown;
	[SerializeField] private GameObject rightUp;
	
	private HashSet<Block> hashSet;
	
	public void Execute(Hex hex,SpecialBlockType blockType, HashSet<Block> withinRangeList)
	{
		if (isLeftDown)
		{
			ShapeCheckUtil.Search(hex, HexWay.LeftDown, withinRangeList);
			ShapeCheckUtil.Search(hex, HexWay.RightUp, withinRangeList);
		}
		else
		{
			ShapeCheckUtil.Search(hex, HexWay.LeftUp, withinRangeList);
			ShapeCheckUtil.Search(hex, HexWay.RightDown, withinRangeList);
		}
		
		hashSet = withinRangeList;
	}
	public async UniTask Anim(Block block)
	{
		var board = GameManager.Instance.Board;

		await board.ShowTargetHighlightAll(hashSet);
		var obj0 = Instantiate(isLeftDown ? leftDown : rightDown);
		var obj1 = Instantiate(isLeftDown ? rightUp : leftUp);
		
		obj0.transform.position = block.transform.position;
		obj1.transform.position = block.transform.position;

		var dir1 = (board.IndexToWorldPos(Hex.GetHexByWay(block.Hex, isLeftDown ? HexWay.LeftDown : HexWay.RightDown)) - block.transform.position).normalized;
		var dir2 = (board.IndexToWorldPos(Hex.GetHexByWay(block.Hex, isLeftDown ? HexWay.RightUp : HexWay.LeftUp)) - block.transform.position).normalized;

		var end0 = obj0.transform.position + (dir1 * 10);
		var end1 = obj1.transform.position + (dir2 * 10);
		
		obj0.transform.DOMove(end0,20).SetEase(Ease.Linear).SetSpeedBased(true).OnComplete(()=>Destroy(obj0));
		obj1.transform.DOMove(end1,20).SetEase(Ease.Linear).SetSpeedBased(true).OnComplete(()=>Destroy(obj1));
	}
}