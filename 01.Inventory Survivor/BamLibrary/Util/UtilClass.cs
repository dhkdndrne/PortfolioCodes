using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Bam.Extensions;

public static class UtilClass
{
	//StringBuilder 꼭 초기화 하고 사용
	private static StringBuilder sb = new();
	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void DebugLog(object msg, LogType logType = LogType.Log)
	{
		sb.Clear();

		switch (logType)
		{
			case LogType.Log:
				sb.Append("<color=#C8C8C8>");
				break;

			case LogType.LogError:
				sb.Append("<color=#a52a2aff>");
				break;

			case LogType.Warning:
				sb.Append("<color=#C18E2B>");
				break;

			case LogType.Try:
				sb.Append("<color=#3F92B5>");
				break;

			case LogType.Success:
				sb.Append("<color=#37D946>");
				break;
		}

		sb.Append("<b> [").Append(msg).Append("] </b></color>");

		Debug.Log(sb.ToString());
	}

	#region 능력치 관련

	public static string AbilityTypeToString(AbilityType abilityType)
	{
		return abilityType switch
		{
			AbilityType.Hp => "현재 체력",
			AbilityType.MaxHp => "최대 체력",
			AbilityType.Damage => "피해량",
			AbilityType.MeleeAtkPower => "근접 공격력",
			AbilityType.ProjectileAtkPower => "투사체 공격력",
			AbilityType.MagicAtkPower => "마법 공격력",
			AbilityType.CriChance => "치명타 확률",
			AbilityType.CriPower => "치명타 공격력",
			AbilityType.DodgeChance => "회피 확률",
			AbilityType.AtkSpeed => "공격 속도",
			AbilityType.MoveSpeed => "이동 속도",
			AbilityType.AtkRange => "사거리",
			AbilityType.Vitality => "활력",
			AbilityType.LifeSteal => "흡혈",
			AbilityType.Recovery => "회복력",
			AbilityType.Revival => "부활",
			AbilityType.Armor => "방어력",
			AbilityType.Luck => "행운",
			AbilityType.GoldGain => "자원 획득",
			AbilityType.ExpGain => "경험치 획득"
		};
	}
	
    #endregion
	
	#region 아이템 등급

	public static string GetItemRankToString(ItemRarity rarity)
	{
		return rarity switch
		{
			ItemRarity.Common => "커먼",
			ItemRarity.UnCommon => "언커먼",
			ItemRarity.Rare => "레어",
			ItemRarity.Unique => "유니크",
			ItemRarity.Legend => "레전드"
		};
	}

    #endregion

	#region 시너지
	public static string SynergyKeywordToString(SynergyKeyword keyword)
	{
		return keyword switch
		{
			SynergyKeyword.Axe => "도끼",
			SynergyKeyword.SpellBook => "마법서",
			SynergyKeyword.Sword => "검",
		};
	}
	

    #endregion
}