using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;

public static class HandleDrawer
{
	// Scene 뷰에서 그릴 대상 StageData (Wave 경로가 포함된 ScriptableObject)
	public static List<WayPoint> wayPoints = new List<WayPoint>();
	public static StageData stageData;

	public static void SetEnable(bool enable, int waveListIndex = -1)
	{
		switch (enable)
		{
			case true:
				SceneView.duringSceneGui += Draw;
				wayPoints = stageData.Wave.wayPoints[waveListIndex].points;
				break;
			case false:
				SceneView.duringSceneGui -= Draw;
				wayPoints = null;
				break;
		}
		
	}

	public static void Draw(SceneView view)
	{
		if (wayPoints == null)
			return;

		// 원하는 구체 크기와 선 색상 설정
		float sphereSize = 0.3f;
		Color sphereColor = Color.green;
		Color lineColor = Color.yellow;

		for (int i = 0; i < wayPoints.Count; i++)
		{
			// 각 WayPoint의 위치를 가져옴 (WayPoint 클래스 내부의 Position 프로퍼티)
			Vector3 pos = wayPoints[i].Position;

			// 구체 그리기
			Handles.color = sphereColor;
			Handles.SphereHandleCap(0, pos, Quaternion.identity, sphereSize, EventType.Repaint);

			// 이전 포인트와 선으로 연결
			if (i > 0)
			{
				Vector3 prevPos = wayPoints[i - 1].Position;
				Handles.color = lineColor;
				Handles.DrawLine(prevPos, pos);
			}
		}

	}
}