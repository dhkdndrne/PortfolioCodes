using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ColorSo List", fileName = "ColorSO List")]
public class ColorDataList : ScriptableObject
{
	[SerializeField] private List<ColorData> colorList = new List<ColorData>();
	public List<ColorData> ColorList => colorList;
}

[System.Serializable]
public class ColorData
{
	public ColorLayer layer;
	public Color color;
}