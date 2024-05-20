using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
	public Item(ItemData data)
	{
		itemData = data;
	}

	[SerializeField] private ItemData itemData;

	[SerializeField] private int amount;

	public int Amount
	{
		get { return amount; }
		set { amount = value; }
	}

	public ItemData Data => itemData;

	/// <summary>
	/// 아이템 개수를 더한 뒤 최대값 보다 많으면 나머지값 리턴
	/// </summary>
	/// <param name="amount"></param>
	/// <returns></returns>
	public int TryAdd(int amount)
	{
		int max = itemData.MaxAmount;
		int sum = this.amount + amount;

		this.amount = Mathf.Clamp(sum, 0, max);
		return sum > max ? sum - max : 0;
	}
}