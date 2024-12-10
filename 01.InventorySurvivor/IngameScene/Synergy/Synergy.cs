using System.Collections.Generic;
using UnityEngine;

public class Synergy
{
	private int id;
	private SynergyKeyword keyword;
	private string synergyName;
	private int[] conditions;
	public SynergyBuff[] Buffs { get; private set; }

	private int appliedCnt;   // 시너지 개수
	private int appliedGrade; // 시너지 적용 인덱스(단계)

	public Synergy(int id, SynergyKeyword keyword, int[] conditions, SynergyBuff[] buffs)
	{
		this.id = id;
		this.keyword = keyword;
		this.conditions = conditions;
		synergyName = UtilClass.SynergyKeywordToString(keyword);
		
		Buffs = buffs;

		appliedGrade = -1;
		appliedCnt = 0;
	}

	public int ID => id;
	public string Name => synergyName;
	public int AppliedGrade => appliedGrade;
	public int[] Conditions => conditions;
	public SynergyKeyword Keyword => keyword;
	/// <summary>
	/// 시너지 단계 받아오기
	/// </summary>
	/// <returns></returns>
	private int GetAppliedGrade()
	{
		for (int i = conditions.Length - 1; i >= 0; i--)
		{
			if (appliedCnt >= conditions[i])
			{
				return i;
			}
		}
		return -1;
	}

	/// <summary>
	/// 시너지 적용 개수 변경
	/// </summary>
	/// <param name="num"></param>
	public void ChangeSynergyCount(int num)
	{
		appliedCnt += num;
		var index = GetAppliedGrade();

		int temp = appliedGrade;
		appliedGrade = index;

		if (temp != index)
		{
			int val = index - temp;

			// 시너지 레벨이 올랐을때
			if (val > 0)
			{
				// 이전 값 빼줌
				if (appliedGrade > 0)
					ApplyPrevBuffValues(appliedGrade - 1);
				
				// 새로운 값 더해줌
				ApplyBuffValues();
			}
			else // 시너지 레벨이 줄어들었을 때
			{
				ApplyPrevBuffValues(appliedGrade + 1);

				// 1레벨도 적용 안될땐 패스
				if (appliedGrade > -1)
					ApplyBuffValues();
			}
		}
	}

	private void ApplyBuffValues()
	{
		foreach (var buff in Buffs)
		{
			PlayerData.Instance.ChangeAbilityValue(buff.AbilityType, buff.Value[appliedGrade]);
		}
	}

	private void ApplyPrevBuffValues(int value)
	{
		foreach (var buff in Buffs)
		{
			PlayerData.Instance.ChangeAbilityValue(buff.AbilityType, -buff.Value[value]);
		}
	}
}
public class SynergyBuff
{
	public AbilityType AbilityType { get; private set; }
	public int[] Value { get; private set; }

	public SynergyBuff(AbilityType type, int[] value)
	{
		AbilityType = type;
		Value = value;
	}
}