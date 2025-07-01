using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class VerticalBlockBehaviour : MonoBehaviour, ISpecialBlockBehaviour
{
	[SerializeField] private GameObject down;
	[SerializeField] private GameObject up;

	private HashSet<Block> hashSet;
	public void Execute(Hex hex,SpecialBlockType blockType, HashSet<Block> withinRangeList)
	{
		hashSet = withinRangeList;
		ShapeCheckUtil.Search(hex, HexWay.Up, withinRangeList);
		ShapeCheckUtil.Search(hex, HexWay.Down, withinRangeList);
	}
	public async UniTask Anim(Block block)
	{
		var board = GameManager.Instance.Board;

		await board.ShowTargetHighlightAll(hashSet);
		
		var obj0 = Instantiate(down);
		var obj1 = Instantiate(up);

		obj0.transform.position = block.transform.position;
		obj1.transform.position = block.transform.position;

		var dir1 = (board.IndexToWorldPos(Hex.GetHexByWay(block.Hex, HexWay.Down)) - block.transform.position).normalized;
		var dir2 = (board.IndexToWorldPos(Hex.GetHexByWay(block.Hex, HexWay.Up)) - block.transform.position).normalized;

		var end0 = obj0.transform.position + (dir1 * 10);
		var end1 = obj1.transform.position + (dir2 * 10);
		
		obj0.transform.DOMove(end0, 20).SetEase(Ease.Linear).SetSpeedBased(true).OnComplete(() => Destroy(obj0));
		obj1.transform.DOMove(end1, 20).SetEase(Ease.Linear).SetSpeedBased(true).OnComplete(() => Destroy(obj1));
	}
}