using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VHierarchy.Libs;

[CustomPropertyDrawer(typeof(RangeGrid))]
public class RangePropertyDrawer : PropertyDrawer
{
	private int col;
	private int row;

	private const int MAX_SIZE = 11;
	private const int CELL_SIZE = 30;
	private const int BOARD_OFFSET = 10;
	private RangeGrid rangeGrid;
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		SetColAndRow(property);

		rangeGrid = (RangeGrid)property.serializedObject.targetObject.GetFieldValue("rangeGrid");
		rangeGrid.Init(col * row);
		var grid = rangeGrid.Grid;

		var originPos = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		EditorGUI.BeginProperty(originPos, label, property);

		Rect board = SetBoard();
		DrawCell(board, grid);

		EditorUtility.SetDirty(property.serializedObject.targetObject);
		EditorGUI.EndProperty();
	}

	private void SetColAndRow(SerializedProperty property)
	{
		col = (int)property.serializedObject.targetObject.GetFieldValue("col");
		row = (int)property.serializedObject.targetObject.GetFieldValue("row");

		if (col is <= 0 or > MAX_SIZE)
		{
			col = col <= 0 ? 1 : col >= MAX_SIZE ? MAX_SIZE : col;
			property.serializedObject.targetObject.SetFieldValue("col", col);
		}
		if (row is <= 0 or > MAX_SIZE)
		{
			row = row <= 0 ? 1 : row >= MAX_SIZE ? MAX_SIZE : row;
			property.serializedObject.targetObject.SetFieldValue("row", row);
		}
	}

	private Rect SetBoard()
	{
		var rect = EditorGUILayout.GetControlRect(true, col * row * BOARD_OFFSET + 20);
		rect.width = col * CELL_SIZE;
		rect.height = row * CELL_SIZE;
		EditorGUI.DrawRect(rect, new Color(0.7f, 0.7f, 0.7f, 1f));
		return rect;
	}

	private void DrawCell(Rect board, List<GridType> grid)
	{
		int index = 0;
		for (int y = 0; y < row; y++)
		{
			for (int x = 0; x < col; x++)
			{
				index = y * col + x;

				Rect tileRect = SplitGrid(board, CELL_SIZE, CELL_SIZE, index);
				var tile = grid[index];

				switch (tile)
				{
					case GridType.CanAttack:
						EditorGUI.DrawRect(new Rect(tileRect.x, tileRect.y, tileRect.width - 1.5f, tileRect.height - 1.5f), Color.red);
						break;

					case GridType.Pivot:
						EditorGUI.DrawRect(new Rect(tileRect.x, tileRect.y, tileRect.width - 1.5f, tileRect.height - 1.5f), Color.green);
						break;

					case GridType.None:
						EditorGUI.DrawRect(new Rect(tileRect.x, tileRect.y, tileRect.width - 1.5f, tileRect.height - 1.5f), Color.black);
						break;
				}
				MouseEvent(tileRect, index, grid);
			}
		}
	}

	private void MouseEvent(Rect tileRect, int index, List<GridType> grid)
	{
		var eventCur = Event.current;
		if (tileRect.Contains(eventCur.mousePosition))
		{
			if (eventCur.type == EventType.MouseDown)
			{
				if (eventCur.button == 0)
				{
					if (grid[index] == GridType.Pivot)
					{
						// 원점 인덱스 해제
						rangeGrid.SetOriginIndex(-1);
					}

					// Attackable이면 None으로, 아니면 Attackable로 토글
					grid[index] = (grid[index] == GridType.CanAttack)
						? GridType.None
						: GridType.CanAttack;
				}
				else if (eventCur.button == 1)
				{
					//origin 있을대
					if (rangeGrid.PivotIndex != -1)
					{
						// 클릭한 셀과 다른곳에 이미 origin이 있을때
						if (index != rangeGrid.PivotIndex)
						{
							grid[rangeGrid.PivotIndex] = GridType.None;
						}
						else
						{
							grid[index] = GridType.None;
							rangeGrid.SetOriginIndex(-1);
							return;
						}
					}

					grid[index] = GridType.Pivot;
					rangeGrid.SetOriginIndex(index);
				}
			}
		}
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
}