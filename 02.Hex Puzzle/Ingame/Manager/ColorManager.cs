using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bam.Singleton;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class ColorManager : Singleton<ColorManager>, IManger
{
	[SerializeField] private ColorDataList colorDataList;
	private Dictionary<ColorLayer, Color> colorDic = new();

	public Color GetColor(ColorLayer layer) => colorDic[layer];

	public void InitManager()
	{
		foreach (var val in colorDataList.ColorList)
		{
			colorDic.Add(val.layer, val.color);
		}
	}

	#if UNITY_EDITOR

	public Color GetColorInEditor(ColorLayer layer)
	{
		return colorDataList.ColorList.Where(x => x.layer == layer).Select(x => x.color).FirstOrDefault();
	}

	#endif
}