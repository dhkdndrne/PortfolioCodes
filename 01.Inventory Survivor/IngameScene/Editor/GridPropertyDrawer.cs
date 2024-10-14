using System.Collections;
using System.Collections.Generic;
using Bam.Extensions;
using UnityEditor;
using UnityEngine;
using VHierarchy.Libs;
using static Define;

 [CustomPropertyDrawer(typeof(ItemGrid))]
 public class GridPropertyDrawer : PropertyDrawer
 {
 	private int boardSize = 10;

 	#if UNITY_EDITOR
 	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
 	{
 		var originPos = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
 		EditorGUI.BeginProperty(originPos, label, property);

 		// if (property.serializedObject.targetObject.name.IsNullOrWhitespace())
 		// 	return;

 		int newBoardSize = ITEM_GRID_MAX_COL * ITEM_GRID_MAX_ROW * boardSize;
 		float cellSize = (float)newBoardSize / ITEM_GRID_MAX_ROW;

 		var board = new Rect(originPos.x, originPos.y, newBoardSize, newBoardSize);

 		Sprite sprite = (Sprite)property.serializedObject.targetObject.GetFieldValue("sprite");
 		if (sprite != null)
 			GUI.DrawTexture(board, sprite.texture, ScaleMode.ScaleToFit);

 		var itemGrid = (ItemGrid)property.serializedObject.targetObject.GetFieldValue("itemGrid");
 		itemGrid.Init();
	    
 		EditorGUI.DrawRect(board, new Color(0.7f, 0.7f, 0.7f, 0.1f));

 		var tempList = itemGrid.GridList;
 		
 		for (int i = 0; i < ITEM_GRID_MAX_ROW * ITEM_GRID_MAX_COL; i++)
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
 		itemGrid.GridList = tempList;
 		EditorGUI.EndProperty();
 	}

 	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
 	{
 		return ITEM_GRID_MAX_COL * ITEM_GRID_MAX_ROW * boardSize;
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