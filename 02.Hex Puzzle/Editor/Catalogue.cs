using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Catalogue
{
	public enum ButtonSize
	{
		Fixed = GUI.ToolbarButtonSize.Fixed,
		FitToContents = GUI.ToolbarButtonSize.FitToContents,
	}

	private ButtonSize buttonSize;
	private List<CatalogueItem> itemList = new List<CatalogueItem>();

	private int catalogueIndex;
	private bool isInit;
	private bool enabled;

	public void OnEnable()
	{
		itemList ??= new List<CatalogueItem>();
	}

	public void OnDisable()
	{
		itemList[catalogueIndex].OnDeSelected();
		enabled = false;
	}

	public void SetEnable(bool value)
	{
		if (enabled == value) return; // 상태가 이미 동일하면 아무 작업도 하지 않음

		if (value)
		{
			OnEnable(); // 활성화 요청
		}
		else
		{
			OnDisable(); // 비활성화 요청
		}

		enabled = value; // 상태 업데이트
	}
	public void Add<T>() where T : CatalogueItem, new()
	{
		var res = new T();
		res.Init();

		itemList.Add(res);
	}

	private void Validate()
	{
		if (itemList.Count == 0) return;

		catalogueIndex = Mathf.Clamp(catalogueIndex, 0, itemList.Count - 1);

		if (!isInit)
		{
			isInit = true;
			itemList[catalogueIndex].OnSelected();
		}
	}

	public void OnGUI(Rect position)
	{
		Validate();
		EditorGUI.BeginChangeCheck();

		var newIndex = GUILayout.Toolbar(catalogueIndex,
			itemList.Select(x => x.CatalogueName).ToArray(), null, (GUI.ToolbarButtonSize)buttonSize);

		if (EditorGUI.EndChangeCheck() && newIndex != catalogueIndex)
		{
			var oldOne = itemList[catalogueIndex];
			var newOne = itemList[newIndex];
			oldOne.IsSelected = false;
			newOne.IsSelected = true;
			oldOne.OnDeSelected();
			newOne.OnSelected();
			catalogueIndex = newIndex;
		}

		itemList[catalogueIndex].DrawUI(position);
	}
}