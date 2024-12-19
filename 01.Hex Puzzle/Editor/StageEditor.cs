using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StageEditor : EditorWindow
{
	private const string StagePath = "Assets/09.Data/Stage/";

	private int stage = 1;

	private int col, row;
	private float colGap, rowGap;

	private Catalogue catalogue;
	[SerializeField] private StageData stageData;
	private Board_Edit board;

	[MenuItem("Stage Editor/Stage Editor")]
	private static void Init()
	{
		var window = GetWindow<StageEditor>();
		window.Show();
		window.minSize = new Vector2(550, 800);
	}

	private void OnEnable()
	{
		catalogue ??= new Catalogue();
		catalogue.Add<BlockCatalogue>();
		catalogue.Add<CellCatalogue>();
		catalogue.Add<TargetCatalogue>();

		board ??= Board_Edit.Instance;
		stageData = null;
		board.DestroyHolder();
	}

	private void OnDisable()
	{
		catalogue.OnDisable();
	}

	private void OnGUI()
	{
		DrawHeader();
		DrawStageDataSection();
		DrawControlButtons();
		DrawCatalogueGUI();
	}

	private void DrawHeader()
	{
		EditorStyles.boldLabel.normal.textColor = Color.yellow;
		EditorGUILayout.Space(10);
		DrawLevelButtons();
		EditorStyles.boldLabel.normal.textColor = Color.white;
	}

	private void DrawStageDataSection()
	{
		GUILayout.Label("Stage Data", EditorStyles.boldLabel);
		stageData = (StageData)EditorGUILayout.ObjectField("Stage Data Asset", stageData, typeof(StageData), false);

		if (stageData == null) return;

		EditorGUILayout.Space(10);

		DrawStageGrid();

		if (HasStageDataChanged())
		{
			UpdateStageGrid();
		}

		if (GUILayout.Button("Close"))
		{
			CloseStageData();
		}

		EditorGUILayout.Space(15);
	}


	private void DrawStageGrid()
	{
		GUILayout.Label("StageData Grid", EditorStyles.boldLabel);

		col = DrawLabeledField("Col", col);
		row = DrawLabeledField("Row", row);
		colGap = DrawLabeledField("Col Gap", colGap);
		rowGap = DrawLabeledField("Row Gap", rowGap);
	}

	private T DrawLabeledField<T>(string label, T value, float labelWidth = 60, float fieldWidth = 80)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(label, GUILayout.Width(labelWidth));

		if (typeof(T) == typeof(int))
		{
			value = (T)(object)EditorGUILayout.IntField((int)(object)value, GUILayout.Width(fieldWidth));
		}
		else if (typeof(T) == typeof(float))
		{
			value = (T)(object)EditorGUILayout.FloatField((float)(object)value, GUILayout.Width(fieldWidth));
		}
		else
		{
			throw new ArgumentException("Unsupported type for labeled field.");
		}

		GUILayout.EndHorizontal();
		return value;
	}


	private void DrawControlButtons()
	{
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Create New Stage", GUILayout.Width(position.width / 2)))
		{
			CreateNewStage();
		}

		if (GUILayout.Button("Load Stage", GUILayout.Width(position.width / 2)))
		{
			LoadStage();
		}
		GUILayout.EndHorizontal();
	}

	private void DrawCatalogueGUI()
	{
		if (stageData == null) return;

		catalogue.SetEnable(Application.isEditor);
		catalogue.OnGUI(position);
	}

	private void DrawLevelButtons()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label("Level", EditorStyles.boldLabel);
		GUILayout.Space(45);

		if (GUILayout.Button("<<", GUILayout.Width(50))) stage = Mathf.Max(stage - 1, 0);
		GUILayout.Space(10);

		stage = EditorGUILayout.IntField(stage, GUILayout.Width(80));
		GUILayout.Space(10);

		if (GUILayout.Button(">>", GUILayout.Width(50))) stage++;
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	private bool HasStageDataChanged()
	{
		return row != stageData.Row || col != stageData.Col ||
		       Math.Abs(rowGap - stageData.RowGap) > float.Epsilon ||
		       Math.Abs(colGap - stageData.ColGap) > float.Epsilon;
	}

	private void UpdateStageGrid()
	{
		stageData.SetRow(row);
		stageData.SetCol(col);
		stageData.RowGap = rowGap;
		stageData.ColGap = colGap;

		stageData.SetBoardSize();
		board.SetBoard();
	}

	private void InitStageGrid()
	{
		col = stageData.Col;
		row = stageData.Row;

		colGap = stageData.ColGap;
		rowGap = stageData.RowGap;
	}
	
	private void CloseStageData()
	{
		stageData = null;
		board.DestroyHolder();
		catalogue.OnDisable();
	}

	private void LoadStage()
	{
		board.DestroyHolder();
		stageData = AssetDatabase.LoadAssetAtPath<StageData>($"{StagePath}Stage_{stage}.asset");

		if (stageData == null)
		{
			Debug.LogError($"Error: 스테이지 {stage}가 없다.");
			return;
		}

		Debug.LogSuccess("로드 성공.");

		InitStageGrid();
		board.SetStageData(stageData);
		board.SetBoard();
	}

	private void CreateNewStage()
	{
		stage = GetNextStageNumber();
		var newStageData = CreateInstance<StageData>();
		string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{StagePath}Stage_{stage}.asset");

		AssetDatabase.CreateAsset(newStageData, assetPath);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		stageData = newStageData;
		board.SetStageData(stageData);
		board.SetBoard();

		Debug.Log($"새로운 stageData 생성 : {assetPath}");
	}

	private int GetNextStageNumber()
	{
		int maxStageNumber = 0;
		string[] stageFiles = AssetDatabase.FindAssets("t:StageData", new[] { StagePath });

		foreach (var guid in stageFiles)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			string fileName = System.IO.Path.GetFileNameWithoutExtension(path);

			if (fileName.StartsWith("Stage_") && int.TryParse(fileName.Substring(6), out int stageNumber))
			{
				maxStageNumber = Mathf.Max(maxStageNumber, stageNumber);
			}
		}

		return maxStageNumber + 1;
	}
}