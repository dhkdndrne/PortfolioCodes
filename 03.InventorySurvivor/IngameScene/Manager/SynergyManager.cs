using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynergyManager : ObjectSingleton<SynergyManager>
{
	private Dictionary<int, Synergy> dic = new Dictionary<int, Synergy>();

	public void Init(Synergy synergy)
	{
		dic.Add(synergy.ID, synergy);
	}

	public void AddSynergy(AttackItemSo item)
	{
		//아이템은 여러개의 시너지를 가지고 있을 수 있음
		foreach (var id in item.synergyIdList)
		{
			dic[id].ChangeSynergyCount(1);
		}
	}

	public void RemoveSynergy(AttackItemSo item)
	{
		//아이템은 여러개의 시너지를 가지고 있을 수 있음
		foreach (var id in item.synergyIdList)
		{
			dic[id].ChangeSynergyCount(-1);
		}
	}

	public Synergy GetSynergy(int id) => dic[id];
	public bool CheckContainSynergy(AttackItemSo item,SynergyKeyword targetKeyword)
	{
		foreach (var id in item.synergyIdList)
		{
			if(dic[id].Keyword == targetKeyword)
				return true;
		}

		return false;
	}
}