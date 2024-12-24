using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class SpecialVerticalBehaviour : MonoBehaviour, ISpecialBlockBehaviour
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
		list.AddRange(PopBlockDataManager.Instance.PopSet);
		
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
		await SpecialBlockBehaviourUtil.ChangeToSpecialBlock(list, hashSet, sBlockData);
	}
}