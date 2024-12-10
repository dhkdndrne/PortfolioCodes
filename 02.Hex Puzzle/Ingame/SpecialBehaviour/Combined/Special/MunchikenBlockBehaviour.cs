using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MunchikenBlockBehaviour : MonoBehaviour, ISpecialBlockBehaviour
{
	public void Execute(Hex hex, HashSet<Block> withinRangeList)
	{
		var board = GameManager.Instance.Board;

		foreach (var b in board.GetBlockEnumerable())
		{
			withinRangeList.Add(b);
		}
	}
	public async UniTask Anim(Block block)
	{
		Debug.Log("먼치킨 애니메이션");
	}
}