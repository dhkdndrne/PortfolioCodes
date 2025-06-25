using System;
using System.Collections;
using System.Collections.Generic;
using Bam.Singleton;
using UnityEngine;
using UnityEngine.Serialization;

public class SpecialBlockCombiner : Singleton<SpecialBlockCombiner>, IManger
{
	[SerializeField] private List<CombineData> combineDataList = new();
	private Dictionary<(SpecialBlockType, SpecialBlockType), (SpecialBlockType type, ISpecialBlockBehaviour behaviour)> dic = new();

	public void Combine(Block sb, Block tb, SpecialBlockType sbType, SpecialBlockType tbType)
	{
		var sortedKey = GetSortedKey(sbType, tbType);

		// 특수 블록이 정의되어있지 않을때 (ex 세로 x 세로 )
		if (!dic.TryGetValue(sortedKey, out var result))
		{
			result.type = sbType;
			result.behaviour = sb.GetComponent<ISpecialBlockBehaviour>();
		}

		Block block = sb;
		PopBlockDataManager.Instance.PopSet.Add(sb);
		PopBlockDataManager.Instance.PopSet.Add(tb);

		if (sb.ColorLayer == ColorLayer.None)
		{
			block = tb;
		}
		PopBlockDataManager.Instance.SpecialBlocks.Add((block.Hex, result.type, result.behaviour));
	}
	public void InitManager()
	{
		foreach (var data in combineDataList)
		{
			dic[GetSortedKey(data.block1, data.block2)] = (data.resultType, data.obj.GetComponent<ISpecialBlockBehaviour>());
		}
	}

	private (SpecialBlockType, SpecialBlockType) GetSortedKey(SpecialBlockType a, SpecialBlockType b)
	{
		return a.CompareTo(b) <= 0 ? (a, b) : (b, a);
	}
}

[Serializable]
public struct CombineData
{
	public SpecialBlockType resultType;
	public SpecialBlockType block1;
	public SpecialBlockType block2;
	public Block obj;
}