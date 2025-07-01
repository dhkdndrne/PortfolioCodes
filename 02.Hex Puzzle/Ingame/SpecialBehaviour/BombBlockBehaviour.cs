using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class BombBlockBehaviour : MonoBehaviour, ISpecialBlockBehaviour
{
	[SerializeField] private int range;
	private Queue<Block> q = new Queue<Block>();
	public void Execute(Hex hex,SpecialBlockType blockType, HashSet<Block> withinRangeList)
	{
		var board = GameManager.Instance.Board;
		q.Clear();
		q.Enqueue(board.GetBlock(hex));
		withinRangeList.Add(board.GetBlock(hex));
		
		int cnt = 1;
		int cycle = range;

		while (cycle > 0)
		{
			while (cnt > 0)
			{
				var b = q.Dequeue();
				cnt--;
				for (int i = 0; i < (int)HexWay.Length; i++)
				{
					var nHex = Hex.GetHexByWay(b.Hex, (HexWay)i);
					if (board.IsIndexOutOfRange(nHex)) continue;
					if (!board.IsValidIndex(nHex)) continue;

					var nb = board.GetBlock(nHex);
					if (nb == null) continue;
					if (nb.BlockData is SpecialBlockData specialBlockData && specialBlockData.SBlockType == SpecialBlockType.Super) continue;
					
					withinRangeList.Add(nb);
					q.Enqueue(nb);
				}
			}
			cnt = q.Count;
			cycle--;
		}
	}
	public async UniTask Anim(Block block)
	{
		q.Enqueue(block);
		await GameManager.Instance.Board.ShowTargetHighlightAll(q.ToHashSet());
	}
}