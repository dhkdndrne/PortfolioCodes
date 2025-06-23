using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Range_", menuName = "Scriptable Objects/RangeData")]
public class RangeData : ScriptableObject
{
	[SerializeField] private int col;
	[SerializeField] private int row;
	
	public GridType[,] GetGridArray()
	{
		GridType[,] grid = new GridType[row,col];

		for (int y = 0; y < row; y++)
		{
			for (int x = 0; x < col; x++)
			{
				//인스펙터에서 지정한 모양대로 나오게 하기
				int revisedRow = (row - 1) - y;
				grid[y, x] = rangeGrid.Grid[revisedRow * col + x];
			}
		}
		return grid;
	}
	
	[SerializeField] private RangeGrid rangeGrid;
}

[Serializable]
public class RangeGrid
{
	[SerializeField] private bool isInit;
	[SerializeField] private List<GridType> grid = new List<GridType>();
	[SerializeField] private int pivotIndex = -1;
	public List<GridType> Grid => grid;
	public int PivotIndex => pivotIndex;

	public void SetOriginIndex(int index) => pivotIndex = index;

	public void Init(int size)
	{
		if (isInit && grid.Count == size)
			return;

		grid = Enumerable.Repeat(GridType.None, size).ToList();

		pivotIndex = -1;
		isInit = true;
	}
}