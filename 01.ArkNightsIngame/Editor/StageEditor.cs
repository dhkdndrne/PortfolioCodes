using UnityEngine;
using UnityEditor;

public class StageEditor : EditorWindow
{
	private StageData targetData;
	private SerializedObject so;
	private Vector2 scroll;

	public void SetTarget(StageData data)
	{
		targetData = data;
		HandleDrawer.stageData = data;
		so = new SerializedObject(data);
	}

	private void OnGUI()
	{
		if (targetData == null)
		{
			EditorGUILayout.HelpBox("먼저 ‘Open Stage Editor’ 버튼을 눌러주세요.", MessageType.Warning);
			return;
		}
		
		DrawHeader();
		
		so.Update();
		
		DrawBasicStageData();

		EditorGUILayout.Space(10);
		
		DrawWaveSection();

		so.ApplyModifiedProperties();
		
		if (GUILayout.Button("Close"))
		{
			targetData = null;
			so = null;
		}
	}

	private void DrawBasicStageData()
	{
		EditorGUILayout.PropertyField(so.FindProperty("characterLimit"));
		EditorGUILayout.PropertyField(so.FindProperty("lifePoint"));
		EditorGUILayout.PropertyField(so.FindProperty("initialCost"));
		EditorGUILayout.PropertyField(so.FindProperty("maxCost"));
		EditorGUILayout.PropertyField(so.FindProperty("costIncreaseTime"));
	}

	private void DrawWaveSection()
	{
		EditorGUILayout.LabelField("Wave", EditorStyles.boldLabel);
		var waveProp = so.FindProperty("wave");
		
		EditorGUILayout.PropertyField(waveProp, true);
	}

	private void DrawHeader()
	{
		EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
		GUILayout.Label("Stage Editor", EditorStyles.boldLabel);
		GUILayout.FlexibleSpace();
		
		if (targetData != null)
			GUILayout.Label($"Editing: <b>{targetData.name}</b>", 
				new GUIStyle(EditorStyles.label) { richText = true });
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space(8);
	}
}