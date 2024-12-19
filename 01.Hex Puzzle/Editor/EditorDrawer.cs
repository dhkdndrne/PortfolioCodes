using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public static class EditorDrawer
{
	private static bool isEnable;
	private static Hex hex;
	public static void SetEnable(bool enable)
	{
		if (!isEnable && enable)
		{
			SceneView.duringSceneGui += Draw;
			isEnable = true;
		}
		else if (isEnable && !enable)
		{
			SceneView.duringSceneGui -= Draw;
			isEnable = false;
		}
	}
	public static void SetHandleIndex(Hex h)
	{
		hex = h;
	}
	public static void Draw(SceneView view)
	{
		var board = Board_Edit.Instance;

		Handles.color = Color.yellow;

		var handlePos = board.IndexToWorldPos(hex);
		Vector3 size = new Vector3(board.ColGap, board.RowGap, 1);
		Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
		Handles.DrawWireCube(handlePos, size);

		view.Repaint();
	}
}