using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TargetCatalogue : CatalogueItem
{
	private const string TARGET_BLOCK_DATA_PATH = "Assets/09.Data/Target/Target_Block";
	private const string COLOR_DATA_PATH = "Assets/09.Data/";

	private const float BUTTON_SPACING = 10f;
	private const float BUTTON_SIZE = 60f;

	private readonly Dictionary<TargetObjectType, List<TargetData>> targetDic = new();
	private TargetData selectedTarget;

	public override string CatalogueName => "타겟";

	public override void Init()
	{
		var targetList = LoadTargets(TARGET_BLOCK_DATA_PATH);

		foreach (TargetObjectType targetObjectType in Enum.GetValues(typeof(TargetObjectType)))
		{
			targetDic.Add(targetObjectType, new List<TargetData>());
			targetDic[targetObjectType].AddRange(targetList.Where(b => b.TargetObjectType == targetObjectType));
		}
	}
	public override void OnSelected()
	{
		selectedTarget = null;
		SceneView.beforeSceneGui += SceneViewRaycast;
		HandleDrawer.SetEnable(true);
	}
	public override void OnDeSelected()
	{
		SceneView.beforeSceneGui -= SceneViewRaycast;
		HandleDrawer.SetEnable(false);
	}

	private List<TargetData> LoadTargets(string path)
	{
		var guids = AssetDatabase.FindAssets("t:TargetData", new[] { path });
		return guids
			.Select(guid => AssetDatabase.LoadAssetAtPath<TargetData>(AssetDatabase.GUIDToAssetPath(guid)))
			.Where(targetData => targetData != null)
			.ToList();
	}
	#region 버튼 그리기

	public override void DrawUI(Rect position)
	{
		EditorGUILayout.BeginVertical();
		DrawButtons(position);
		EditorGUILayout.EndVertical();
	}
	protected override void DrawButtons(Rect position)
	{
		var buttonDimensions = CalculateButtonDimensions(position.width, position.height, targetDic.Count);
		int rowCount = 0;

		EditorGUILayout.BeginHorizontal();
		DrawDeleteButton(buttonDimensions);
		foreach (var list in targetDic)
		{
			if (list.Value.Count == 0)
				continue;

			foreach (var targetData in list.Value)
			{
				var buttonContent = new GUIContent { image = targetData.GetSprite().texture };

				SetButtonBackgroundColor(targetData);
				if (GUILayout.Button(buttonContent, CreateButtonStyle(buttonDimensions.width, buttonDimensions.height)))
				{
					selectedTarget = targetData;
				}
			}
			rowCount = HandleRowLayout(rowCount, buttonDimensions.buttonsPerRow);
		}
		EditorGUILayout.EndHorizontal();
		GUI.backgroundColor = Color.white;
	}

	private void DrawDeleteButton((float width, float height, int ) buttonDimensions)
	{
		var buttonContent = new GUIContent { text = "X" };
		SetButtonBackgroundColor(null);

		if (GUILayout.Button(buttonContent, CreateButtonStyle(buttonDimensions.width, buttonDimensions.height)))
		{
			selectedTarget = null;
			Debug.Log("null");
		}
	}
	private (float width, float height, int buttonsPerRow) CalculateButtonDimensions(float availableWidth, float availableHeight, int itemCount)
	{
		float buttonWidth = Mathf.Min(BUTTON_SIZE, availableWidth / Mathf.Ceil(Mathf.Sqrt(itemCount)));
		float buttonHeight = Mathf.Min(BUTTON_SIZE, availableHeight / Mathf.Ceil(Mathf.Sqrt(itemCount)));
		int buttonsPerRow = Mathf.FloorToInt(availableWidth / (buttonWidth + BUTTON_SPACING));

		return (buttonWidth, buttonHeight, buttonsPerRow);
	}

	private void SetButtonBackgroundColor(TargetData targetData)
	{
		GUI.backgroundColor = selectedTarget == targetData ? Color.green : Color.white;
	}

	private GUIStyle CreateButtonStyle(float width, float height)
	{
		return new GUIStyle(GUI.skin.button)
		{
			fixedWidth = width,
			fixedHeight = height,
			imagePosition = ImagePosition.ImageAbove
		};
	}

	private int HandleRowLayout(int rowCount, int buttonsPerRow)
	{
		rowCount++;
		if (rowCount >= buttonsPerRow)
		{
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			return 0;
		}
		return rowCount;
	}

#endregion

	private void SceneViewRaycast(SceneView view)
	{
		var board = Board_Edit.Instance;
		if (board?.stageData is null) return;

		Vector3 worldPos = Bam.Extensions.Extensions.ScreenToWorldPoint(view);
		Hex hex = board.WorldPosToHex(worldPos);
		if (board.IsIndexOutOfRange(hex)) return;

		HandleMouseInput(board, hex);
		HandleDrawer.SetHandleIndex(hex, 0);
	}
	private void HandleMouseInput(Board_Edit board, Hex hex)
	{
		var e = Event.current;
		if (e.type == EventType.MouseDown)
		{
			int value = (e.button == 0) ? 1 : -1;

			// 왼클릭인데 selectedTarget이 null → 타겟 삭제
			if (selectedTarget == null && e.button == 0)
			{
				board.SetTarget(null, hex, 0);
			}
			else
			{
				// 왼클릭(1)이면 블록 생성/HP 증가, 우클릭(-1)이면 HP 감소/블록 제거
				board.SetTarget(selectedTarget, hex, value);
			}

			EditorUtility.SetDirty(board.stageData);
			e.Use();
		}
	}
}