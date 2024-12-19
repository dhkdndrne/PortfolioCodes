using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoomerangBehaviour : MonoBehaviour, ISpecialBlockBehaviour
{
	private readonly string POOLINGKEY = "BoomerangProjectile";
	public void Execute(Hex hex,SpecialBlockType blockType, HashSet<Block> withinRangeList)
	{
		var board = GameManager.Instance.Board;
		int idx = Random.Range(0, 2);

		for (int cnt = 0; cnt < 3; cnt++, idx += 2)
		{
			var nHex = Hex.GetHexByWay(hex, (HexWay)idx);
			if(board.IsIndexOutOfRange(nHex)) continue;
			if(!board.IsValidIndex(nHex)) continue;
			
			withinRangeList.Add(board.GetBlock(nHex));
		}

		var projectile = ObjectPoolManager.Instance.Spawn(POOLINGKEY);
		projectile.transform.position = transform.position;
		
		//projectile.GetComponent<BoomerangProjectile>().Init(block.ColorLayer);
	}
	public async UniTask Anim(Block block)
	{
		return;
	}
}