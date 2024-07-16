using System;
using System.Collections.Generic;
using System.Linq;
using Bam.Extensions;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Skill_Buff", fileName = "New Buff Skill")]
public class BuffSkill : Skill
{
	[SerializeField] private ApplyTarget applyTarget;
	[SerializeField] private SynergyKeyword targetKeyword;
	[SerializeField] private List<BuffToken> buffList = new List<BuffToken>();
	
	/// <summary>
	/// 버프값 계산
	/// </summary>
	/// <param name="neighborHash"></param>
	public void AddBuffValue(HashSet<InventoryItem> neighborHash)
	{
		ResetBuff();
		
		foreach (var neighbor in neighborHash)
		{
			if (neighbor.ItemSo.ItemType is ItemType.Weapon)
			{
				var weaponSo = neighbor.ItemSo as AttackItemSo;

				if (SynergyManager.Instance.CheckContainSynergy(weaponSo, targetKeyword))
				{
					foreach (var buff in buffList)
						buff.value += buff.increaseAmount;
				}
			}
		}
	}

	/// <summary>
	/// 이웃 아이템 빠졌을떄 버프값 감소 계산
	/// </summary>
	/// <param name="item"></param>
	public void ReduceBuffValue(InventoryItem item)
	{
		if (item.ItemSo.ItemType is ItemType.Weapon)
		{
			var weaponSo = item.ItemSo as AttackItemSo;

			if (SynergyManager.Instance.CheckContainSynergy(weaponSo, targetKeyword))
			{
				foreach (var buff in buffList)
					buff.value -= buff.increaseAmount;
			}
		}
	}

	public void ResetBuff()
	{
		foreach (var buff in buffList)
			buff.value = 0;
	}

	public IEnumerable<string> GetAppliedBuffText()
	{
		foreach (var buff in buffList)
		{
			if (buff.value == 0)
				continue;

			string abilityName = UtilClass.AbilityTypeToString(buff.abilityType);

			bool isMinus = buff.value < 0;
			string value = isMinus ? buff.value.ToString() : $"+{buff.value}";
			StringColor color = isMinus ? StringColor.Red : StringColor.Green;

			yield return ColorManager.Instance.GetColorString(Extensions.GetTextAppendLine(abilityName, value), color);
		}
	}

	public IEnumerable<BuffToken> GetBuff()
	{
		foreach (var buff in buffList)
		{
			yield return buff;
		}
	}
	
	public int GetBuffValue(AbilityType abilityType)
	{
		return buffList.Where(x => x.abilityType == abilityType).Select(x => x.value).FirstOrDefault();
	}

	public bool TargetIsMe() => applyTarget == ApplyTarget.Me;
	
	private enum ApplyTarget
	{
		Me,
		Other
	}
	
	[Serializable]
	public class BuffToken
	{
		public AbilityType abilityType;
		public int increaseAmount;
		[HideInInspector] public int value;
	}
}