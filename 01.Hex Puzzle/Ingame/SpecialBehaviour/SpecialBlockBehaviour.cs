using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SpecialBlockBehaviour : MonoBehaviour, ISpecialBlockBehaviour
{
	private HashSet<Block> hashSet;

	public void Execute(Hex hex,SpecialBlockType blockType, HashSet<Block> withinRangeList)
	{
		Block targetBlock = PopBlockDataManager.Instance.SwapSet.First();
		if (targetBlock == null) return;

		var board = GameManager.Instance.Board;
		withinRangeList.Add(PopBlockDataManager.Instance.PopSet.First());
		
		foreach (var b in board.GetBlockEnumerable())
		{
			if (b.ColorLayer != targetBlock.ColorLayer) continue;
			withinRangeList.Add(b);
		}
		
		hashSet = withinRangeList;
	}

	public async UniTask Anim(Block block)
	{
		await GameManager.Instance.Board.ShowTargetHighlightAnim(hashSet);
	}
}

public static class SpecialBlockBehaviourUtil
{
	public static async UniTask ChangeBlockToSlash(Board board, List<Block> list, HashSet<Block> hashSet, SpecialBlockData sBlockData)
	{
		foreach (var b in list)
		{
			var sBlock = BlockSpawner.Instance.SpawnSpecialBlock(new ReservedSBlockData(sBlockData, b.ColorLayer, b.Hex, null));
			board.RemoveBlock(b.Hex);
			board.SetBlock(b.Hex, sBlock);
			sBlock.transform.position = board.IndexToWorldPos(b.Hex);
			hashSet.Add(sBlock);

			await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
		}
	}
}