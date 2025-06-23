using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StageData))]
public class StageDataInspector : Editor
{
	SerializedProperty characterLimitProp;
	SerializedProperty lifePointProp;
	SerializedProperty initialCostProp;
	SerializedProperty maxCostProp;
	SerializedProperty costIncreaseTimeProp;

	void OnEnable()
	{
		characterLimitProp   = serializedObject.FindProperty("characterLimit");
		lifePointProp        = serializedObject.FindProperty("lifePoint");
		initialCostProp      = serializedObject.FindProperty("initialCost");
		maxCostProp          = serializedObject.FindProperty("maxCost");
		costIncreaseTimeProp = serializedObject.FindProperty("costIncreaseTime");
	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		// 1) Draw only the basic fields, all editable
		EditorGUILayout.PropertyField(characterLimitProp);
		EditorGUILayout.PropertyField(lifePointProp);
		EditorGUILayout.PropertyField(initialCostProp);
		EditorGUILayout.PropertyField(maxCostProp);
		EditorGUILayout.PropertyField(costIncreaseTimeProp);

		serializedObject.ApplyModifiedProperties();

		EditorGUILayout.Space();

		//StageEditor 창으로 열기 버튼
		if (GUILayout.Button("🔧 Open Stage Editor"))
		{
			StageEditor window = EditorWindow.GetWindow<StageEditor>("Stage Editor");
			window.SetTarget((StageData)target);
		}

		EditorGUILayout.HelpBox(
			"⚠️ Wave는 이 인스펙터에서 직접 수정 불가합니다.\n"
			+ "    ‘Open Stage Editor’ 버튼을 눌러주세요.",
			MessageType.Info
		);
	}
}