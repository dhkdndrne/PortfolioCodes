using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[CreateAssetMenu(menuName = "SO/InventorySetting",fileName = "New InventorySetting SO")]
public class InventorySetting : ScriptableObject
{
	[SerializeField] private int col, row;
	[Tooltip("레벨업당 슬롯 해금 개수")]
	[SerializeField] private int unlockSlotPerLevelUp;
	[SerializeField] private InventoryLockGrid inventoryLockGrid;
	
	public int Col => col;
	public int Row => row;
	public int UnlockSlotPerLevelUp => unlockSlotPerLevelUp;
	public InventoryLockGrid InventoryLockGrid => inventoryLockGrid;
}

[System.Serializable]
public class InventoryLockGrid
{
	[SerializeField] private List<int> lockList = new List<int>();
	[SerializeField] private bool isInit;
	[SerializeField] private int col, row;
	private int[,] grids;
	
	public List<int> LockList
	{
		get { return lockList; }
		set { lockList = value; }
	}
	
	public int[,] Grids
	{
		get
		{
			if (grids == null)
				ListToArray();
			
			return grids;
		}
	}
	
	private void ListToArray()
	{
		grids = new int[row, col];
		
		for (int y = 0; y < row; y++)
		{
			for (int x = 0; x < col; x++)
			{
				//인스펙터에서 지정한 모양대로 나오게 하기
				int revisedRow = (row - 1) - y;
				grids[y, x] = lockList[revisedRow * col + x];
			}
		}
	}
	
	public void Init(int col,int row)
	{
		if (isInit && lockList.Count == col * row )
			return;
		
		lockList.Clear();
		
		this.col = col;
		this.row = row;
		
		int size = col * row;
		for (int i = 0; i < size; i++)
			lockList.Add(0);
		
		Debug.Log("InventoryLock 초기화");
		isInit = true;
	}
}
