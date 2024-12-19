using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class SpecialSlashBehaviour : MonoBehaviour, ISpecialBlockBehaviour
{ 
	[Header("변환할 아이템")]
	[SerializeField] private SpecialBlockData sBlockData;
	
	private List<Block> list = new();
	private HashSet<Block> hashSet;
	
	public void Execute(Hex hex,SpecialBlockType blockType, HashSet<Block> withinRangeList)
	{
		list.Clear();
		var board = GameManager.Instance.Board;
		var colorLayer = board.GetBlock(hex).ColorLayer;
		
		foreach (var b in board.GetBlockEnumerable())
		{
			if (b.ColorLayer == colorLayer && b.BlockData is not SpecialBlockData)
			{
				list.Add(b);
			}
		}
		hashSet = withinRangeList;
	}
	
	public async UniTask Anim(Block block)
	{
		Board board = GameManager.Instance.Board;

		var task1 = board.ShowTargetHighlightAnim(list.ToHashSet());
		var task2 = SpecialBlockBehaviourUtil.ChangeBlockToSlash(board, list, hashSet, sBlockData);
			
		await UniTask.WhenAll(task1, task2);
	}
	
}