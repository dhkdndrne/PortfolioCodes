using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class ColorManager
{
	private static ColorDataList colorDataList;
	private static Dictionary<ColorLayer, Color> colorDic = new();
	private const string COLOR_DATA_PATH = "Assets/09.Data/ColorSO List.asset";
	public static Color GetColor(ColorLayer layer)
	{
		if (colorDic.Count == 0)
		{
			colorDataList = AssetDatabase.LoadAssetAtPath<ColorDataList>(COLOR_DATA_PATH);
			foreach (var val in colorDataList.ColorList)
			{
				colorDic.Add(val.layer, val.color);
			}
		}
		
		return colorDic[layer];
	}
	
	#if UNITY_EDITOR

	public static Color GetColorInEditor(ColorLayer layer)
	{
		if (colorDic.Count == 0)
		{
			colorDataList = AssetDatabase.LoadAssetAtPath<ColorDataList>(COLOR_DATA_PATH);
			foreach (var val in colorDataList.ColorList)
			{
				colorDic.Add(val.layer, val.color);
			}
		}
		
		return colorDataList.ColorList.Where(x => x.layer == layer).Select(x => x.color).FirstOrDefault();
	}

	#endif
}