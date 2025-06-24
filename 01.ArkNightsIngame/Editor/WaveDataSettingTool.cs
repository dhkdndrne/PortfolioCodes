using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class WaveDataSettingTool
{
	// 버튼에서 세팅해 주는 값들
	public static SerializedObject currentSettingSerializedObject;
	public static string currentSettingPropertyPath;
	public static int currentWayPointIndex = -1;

	// Tile 레이어만 레이캐스트
	private static readonly int tileMask = LayerMask.GetMask("Tile");

	static WaveDataSettingTool()
	{
		SceneView.duringSceneGui += OnSceneGUI;
	}

	private static void OnSceneGUI(SceneView sceneView)
	{
		// 대상이 없으면 조기 리턴
		if (currentSettingSerializedObject == null
		    || string.IsNullOrEmpty(currentSettingPropertyPath)
		    || currentWayPointIndex < 0)
			return;

		Event e = Event.current;
		if (e.type != EventType.MouseDown) return;

		bool isLeftClick = (e.button == 0);

		// 씬 뷰 클릭 → 타일 레이캐스트
		Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
		if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, tileMask))
			return;

		if (!hit.collider.GetComponent<Tile>().TileType.HasFlag(TileType.Road))
			return;
		
		// 클릭 위치
		Vector3 tilePos = hit.collider.transform.position;
		tilePos.y += hit.collider.transform.localScale.y * 0.7f;

		// WayPointData 요소 찾기
		var wpDataProp = currentSettingSerializedObject.FindProperty(currentSettingPropertyPath);
		if (wpDataProp == null) return;

		// points 리스트
		var pointsProp = wpDataProp.FindPropertyRelative("points");
		if (pointsProp == null) return;

		currentSettingSerializedObject.Update();

		if (isLeftClick)
		{
			// 새 WayPoint 추가
			int insertIndex = pointsProp.arraySize;
			pointsProp.InsertArrayElementAtIndex(insertIndex);
			var newPt = pointsProp.GetArrayElementAtIndex(insertIndex);

			var posProp = newPt.FindPropertyRelative("position");
			if (posProp != null)
				posProp.vector3Value = tilePos;
		}
		else
		{
			//가까운 WayPoint 삭제
			for (int i = 0; i < pointsProp.arraySize; i++)
			{
				var ptProp = pointsProp.GetArrayElementAtIndex(i);
				var posProp = ptProp.FindPropertyRelative("position");
				if (posProp == null) continue;

				if (Vector3.Distance(posProp.vector3Value, tilePos) < 0.5f)
				{
					pointsProp.DeleteArrayElementAtIndex(i);
					break;
				}
			}
		}

		currentSettingSerializedObject.ApplyModifiedProperties();
		e.Use();
	}
}