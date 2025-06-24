using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Wave))]
public class WavePropertyDrawer : PropertyDrawer
{
	// 키: "wave.wayPoints.Array.data[i]", 값: 토글 상태
	static Dictionary<string, bool> settingMode = new Dictionary<string, bool>();
	static string currentKey;

	public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
	{
		EditorGUI.BeginProperty(pos, label, prop);

		float y = pos.y;
		float lh = EditorGUIUtility.singleLineHeight;
		float pad = 2f;

		//  Wave Foldout
		prop.isExpanded = EditorGUI.Foldout(
			new Rect(pos.x, y, pos.width, lh),
			prop.isExpanded, label, true
		);
		y += lh + 4f;

		if (prop.isExpanded)
		{
			EditorGUI.indentLevel++;

			// WayPoints 리스트 헤더 + Size
			var wpList = prop.FindPropertyRelative("wayPoints");
			wpList.isExpanded = EditorGUI.Foldout(
				new Rect(pos.x, y, pos.width, lh),
				wpList.isExpanded, "WayPoints", true
			);
			y += lh + pad;

			if (wpList.isExpanded)
			{
				EditorGUI.indentLevel++;
				// Size 필드
				var sizeProp = wpList.FindPropertyRelative("Array.size");
				Rect sizeR = new Rect(pos.x, y, pos.width, lh);
				EditorGUI.PropertyField(sizeR, sizeProp, new GUIContent("Size"));
				y += lh + 4f;

				// 각 Element
				for (int i = 0; i < wpList.arraySize; i++)
				{
					var elem = wpList.GetArrayElementAtIndex(i);

					// Foldout
					Rect headerR = new Rect(pos.x, y, pos.width, lh);
					elem.isExpanded = EditorGUI.Foldout(
						headerR, elem.isExpanded, $"Element {i}", true
					);
					y += lh + pad;

					if (elem.isExpanded)
					{
						EditorGUI.indentLevel++;

						// index 필드
						var indexProp = elem.FindPropertyRelative("index");
						Rect idxR = new Rect(pos.x, y, pos.width, lh);
						EditorGUI.PropertyField(idxR, indexProp);
						y += lh + pad;

						// points 리스트
						var ptsProp = elem.FindPropertyRelative("points");
						float ptsH = EditorGUI.GetPropertyHeight(ptsProp, true);
						Rect ptsR = new Rect(pos.x, y, pos.width, ptsH);
						EditorGUI.PropertyField(ptsR, ptsProp, true);
						y += ptsH + pad;

						// 버튼: Element 내부 마지막
						string key = prop.propertyPath + $".wayPoints.Array.data[{i}]";
						bool isOn = settingMode.TryGetValue(key, out var v) && v;
						Color prev = GUI.backgroundColor;
						GUI.backgroundColor = isOn ? Color.red : Color.white;

						Rect btnR = new Rect(pos.x, y, pos.width, lh);
						if (GUI.Button(btnR, "웨이포인트 설정 모드"))
						{
							// 이전 활성화 요소 해제
							if (!string.IsNullOrEmpty(currentKey) && currentKey != key)
								settingMode[currentKey] = false;

							// 토글
							isOn = !isOn;
							settingMode[key] = isOn;
							currentKey = isOn ? key : null;

							if (isOn)
							{
								WaveDataSettingTool.currentSettingSerializedObject = prop.serializedObject;
								WaveDataSettingTool.currentSettingPropertyPath = key;
								WaveDataSettingTool.currentWayPointIndex = i;
								HandleDrawer.SetEnable(true, i);
							}
							else
							{
								WaveDataSettingTool.currentSettingSerializedObject = null;
								WaveDataSettingTool.currentWayPointIndex = -1;
								HandleDrawer.SetEnable(false);
							}
						}
						GUI.backgroundColor = prev;
						y += lh + 6f;

						EditorGUI.indentLevel--;
					}
				}

				EditorGUI.indentLevel--;
			}

			// WaveGroups 리스트
			var wgProp = prop.FindPropertyRelative("waveList");
			float wgH = EditorGUI.GetPropertyHeight(wgProp, true);
			Rect wgR = new Rect(pos.x, y, pos.width, wgH);
			EditorGUI.PropertyField(wgR, wgProp, new GUIContent("Wave Groups"), true);
			y += wgH + 4f;

			EditorGUI.indentLevel--;
		}

		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
	{
		float h = EditorGUIUtility.singleLineHeight + 4f;
		if (!prop.isExpanded) return h;

		float lh = EditorGUIUtility.singleLineHeight;
		float pad = 2f;

		// WayPoints 헤더
		h += lh + pad;
		var wpList = prop.FindPropertyRelative("wayPoints");
		if (wpList.isExpanded)
		{
			// Size
			h += lh + 4f;
			// 각 Element 높이
			for (int i = 0; i < wpList.arraySize; i++)
			{
				var elem = wpList.GetArrayElementAtIndex(i);
				// Foldout 헤더
				h += lh + pad;
				if (elem.isExpanded)
				{
					// index
					h += lh + pad;
					// points
					h += EditorGUI.GetPropertyHeight(elem.FindPropertyRelative("points"), true) + pad;
					// button
					h += lh + 6f;
				}
			}
		}
		// WaveGroups 높이
		var wgProp = prop.FindPropertyRelative("waveList");
		h += EditorGUI.GetPropertyHeight(wgProp, true) + 4f;

		return h;
	}
}