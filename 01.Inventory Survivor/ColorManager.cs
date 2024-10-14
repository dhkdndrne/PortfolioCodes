using System;
using System.Collections.Generic;
using System.Text;
using Bam.Singleton;
using UnityEngine;

public class ColorManager : DontDestroySingleton<ColorManager>
{
	[SerializeField] private RarityColorToken[] rarityColors;
	[field:SerializeField ] public InventoryColor InvenColor { get; private set; }
	
	private Dictionary<ItemRarity, Color> rarityColorDic;
	
	public Color GetItemRarityColor(ItemRarity rarity) => rarityColorDic[rarity];
	private StringBuilder sb;
	#region ColorHex
	
	public readonly string HEX_RED = "FF0000";
	public readonly string HEX_GREEN = "44FF00";
	public readonly string HEX_BLACK = "A6A6A6";
	public readonly string HEX_SYNERGYNAME = "FBFFC0";
	public readonly string HEX_WHITE = "FFFFFF";
    #endregion
	
	
	private void Start()
	{
		Init();
	}

	private void Init()
	{
		sb = new StringBuilder();
		rarityColorDic = new Dictionary<ItemRarity, Color>();
		
		foreach (var token in rarityColors)
		{
			rarityColorDic.Add(token.rarity,token.color);
		}
	}

	public string GetColorString(string s,StringColor color)
	{
		string hexColor = color switch
		{
			StringColor.Red => HEX_RED,
			StringColor.Green => HEX_GREEN,
			StringColor.Black => HEX_BLACK,
			StringColor.Yellow => HEX_SYNERGYNAME,
			StringColor.White => HEX_WHITE,
		};
		
		return $"<color=#{hexColor}>{s}</color>";
	}
	
	public string GetColorAbilityValue(AbilityType abilityType, float originValue, float changedValue)
	{
		sb.Clear();

		bool check = abilityType is AbilityType.CriChance or AbilityType.CriPower;
		string s = check ? $"{changedValue}%" : changedValue.ToString();
		
		if ((int)changedValue == (int)originValue)
		{
			sb.Append(GetColorString(s,StringColor.White));
		}
		else if (changedValue < originValue)
		{
			sb.Append(GetColorString(s,StringColor.Red));
		}
		else
		{
			sb.Append(GetColorString(s,StringColor.Green));
		}
		
		return sb.ToString();
	}
    
	[Serializable]
	public struct InventoryColor
	{
		[SerializeField] private Color initalColor;
		[SerializeField] private Color onHoverItem;
		[SerializeField] private Color onHoverItemOverlapError;
		[SerializeField] private Color onHoverEmptyCell;

		public Color OnHoverItem => onHoverItem;
		public Color OnHoverItemOverlapError => onHoverItemOverlapError;
		public Color OnHoverEmptyCell => onHoverEmptyCell;
		public Color OnInitalColor => initalColor;
	}
	
	[Serializable]
	private struct RarityColorToken
	{
		public ItemRarity rarity;
		public Color color;
	}
}
