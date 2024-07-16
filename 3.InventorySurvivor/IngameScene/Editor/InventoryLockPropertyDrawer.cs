using System.Collections;
using System.Collections.Generic;
using Bam.Extensions;
using UnityEditor;
using UnityEngine;
using VHierarchy.Libs;
using static Define;

[CustomPropertyDrawer(typeof(InventoryLockGrid))]
public class InventoryLockPropertyDrawer : PropertyDrawer
{
	private readonly int Board_SIZE = 400;
	#if UNITY_EDITOR
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var originPos = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		EditorGUI.BeginProperty(originPos, label, property);

		int col = (int)property.serializedObject.targetObject.GetFieldValue("col");
		int row = (int)property.serializedObject.targetObject.GetFieldValue("row");
		float cellSize = (float)Board_SIZE / row;
		
		var board = new Rect(originPos.x ,originPos.y, Board_SIZE, Board_SIZE);
		var lockGrid = (InventoryLockGrid)property.serializedObject.targetObject.GetFieldValue("inventoryLockGrid");
		lockGrid.Init(col,row);
		
		EditorGUI.DrawRect(board, new Color(0.8f, 0.8f, 0.8f, 0.1f));
		
		var tempList = lockGrid.LockList;
 		
		for (int i = 0; i < row * col; i++)
		{
			Rect tileRect = SplitGrid(board, cellSize, cellSize, i);
			Color color = tempList[i] == 0 ? new Color(0f, 0f, 0f, 0.3f) : new Color(0.3f, 0.7f, 0.3f, 0.3f);
			EditorGUI.DrawRect(new Rect(tileRect.x, tileRect.y, tileRect.width - 1.5f, tileRect.height - 1.5f), color);
			if (tileRect.Contains(Event.current.mousePosition))
			{
				EditorGUI.DrawRect(new Rect(tileRect.x, tileRect.y, tileRect.width - 1.5f, tileRect.height - 1.5f), new Color(.45f, .45f, .45f, 0.3f));
				if (Event.current.type == EventType.MouseDown)
				{
					tempList[i] = tempList[i] == 1 ? 0 : 1;
					Event.current.Use();
				}
			}
		}
 		
		EditorUtility.SetDirty(property.serializedObject.targetObject);
		lockGrid.LockList = tempList;
		EditorGUI.EndProperty();
	}
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return Board_SIZE;
	}
	private Rect SplitGrid(Rect rect, float width, float height, int index)
	{
		int num1 = (int)((double)rect.width / (double)width);
		int num2 = num1 > 0 ? num1 : 1;
		int num3 = index % num2;
		int num4 = index / num2;
		rect.x += (float)num3 * width;
		rect.y += (float)num4 * height;
		rect.width = width;
		rect.height = height;
		return rect;
	}
	#endif
}
