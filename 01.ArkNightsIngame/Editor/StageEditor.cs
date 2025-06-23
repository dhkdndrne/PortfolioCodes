// Assets/Editor/StageEditor.cs
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

		// 1) 헤더, 레벨 컨트롤 등…
		DrawHeader();

		// 2) 실제 수정 파트는 모두 이 _so 로만!
		so.Update();

		// → StageData 기본 프로퍼티 (characterLimit, lifePoint…)
		DrawBasicStageData();

		EditorGUILayout.Space(10);

		// → Wave 섹션
		DrawWaveSection();

		so.ApplyModifiedProperties();

		// 3) 닫기
		if (GUILayout.Button("Close"))
		{
			targetData = null;
			so = null;
		}
	}

	private void DrawBasicStageData()
	{
		// 예시: 5개 프로퍼티만 뽑아서 편집
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
		// 에디터 창 제목
		GUILayout.Label("Stage Editor", EditorStyles.boldLabel);
		GUILayout.FlexibleSpace();
		
		// 현재 편집 중인 SO 이름 표시
		if (targetData != null)
			GUILayout.Label($"Editing: <b>{targetData.name}</b>", 
				new GUIStyle(EditorStyles.label) { richText = true });
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space(8);
	}
}