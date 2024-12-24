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
		var board = GameManager.Instance.Board;
		
		foreach (var b in hashSet)
		{
			var cell = board.GetCell(b.Hex);
			cell.Shadow.SetActive(false);
			await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
		}
	}
}

public static class SpecialBlockBehaviourUtil
{
	public static async UniTask ChangeToSpecialBlock(List<Block> list, HashSet<Block> hashSet, SpecialBlockData sBlockData)
	{
		var board = GameManager.Instance.Board;
		
		for (int i = 0; i < list.Count; i++)
		{
			var b = list[i];

			var cell = board.GetCell(b.Hex);
			cell.Shadow.SetActive(false);

			if (i < 2) // 0,1 번째 인덱스는 조합에 사용된 특수블록들이므로 하이라이트만 비추고 변환은 하지않기위해서 continue
				continue;
			
			var sBlock = BlockSpawner.Instance.SpawnSpecialBlock(new ReservedSBlockData(sBlockData, b.ColorLayer, b.Hex, null));
			board.RemoveBlock(b.Hex);
			board.SetBlock(b.Hex, sBlock);
			sBlock.transform.position = board.IndexToWorldPos(b.Hex);
			hashSet.Add(sBlock);

			await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
		}
	}
}