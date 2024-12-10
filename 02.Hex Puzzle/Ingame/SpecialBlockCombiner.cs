using System;
using System.Collections;
using System.Collections.Generic;
using Bam.Singleton;
using UnityEngine;

public class SpecialBlockCombiner : Singleton<SpecialBlockCombiner>, IManger
{
	private Dictionary<SpecialBlockType, ISpecialBlockBehaviour> dic = new();
	[SerializeField] private List<CombineData> combineDataList = new();

	public void Combine(Block sb, Block tb, SpecialBlockType sbType, SpecialBlockType tbType)
	{
		SpecialBlockType newBlockType = (SpecialBlockType)((int)sbType * (int)tbType);

		// 특수 블록이 정의되어있지 않을때 (ex 세로 x 세로 )
		if (!Enum.IsDefined(typeof(SpecialBlockType), newBlockType))
		{
			newBlockType = sbType;
		}

		if (!dic.TryGetValue(newBlockType, out ISpecialBlockBehaviour blockBehaviour))
		{
			blockBehaviour = sb.GetComponent<ISpecialBlockBehaviour>();
		}
		
		Block block = sb;
		PopBlockDataManager.Instance.PopSet.Add(sb);
		PopBlockDataManager.Instance.PopSet.Add(tb);

		if (sb.ColorLayer == ColorLayer.None)
		{
			block = tb;
			//GameManager.Instance.Board.RemoveBlock(sb.Hex);
		}
		//else GameManager.Instance.Board.RemoveBlock(tb.Hex);

		PopBlockDataManager.Instance.CombinedSBlock = (block.Hex, newBlockType,blockBehaviour);
	}
	public void InitManager()
	{
		foreach (var data in combineDataList)
		{
			dic.Add(data.BlockType, data.obj.GetComponent<ISpecialBlockBehaviour>());
		}
	}
}

[Serializable]
public struct CombineData
{
	public SpecialBlockType BlockType;
	public Block obj;
}