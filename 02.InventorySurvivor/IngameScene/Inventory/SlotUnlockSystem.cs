using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotUnlockSystem
{
	private HashSet<InventorySlot> canUnlockSlotList; //해금 가능한 모든슬롯 담은 리스트
	private List<InventorySlot> randomUnlockSlots; //랜덤으로 선택된 해금 가능한 슬롯 리스트

	private int col, row,unLockcnt;

	public void Init(InventorySetting setting)
	{
		canUnlockSlotList = new HashSet<InventorySlot>();
		randomUnlockSlots = new List<InventorySlot>();

		col = setting.Col;
		row = setting.Row;
		unLockcnt = setting.UnlockSlotPerLevelUp;
	}
	
	public void GetAllUnlockableSlot()
	{
		var inventorySlots = Inventory.Instance.InventorySlots;
		
		for (int y = 0; y < row; y++)
		{
			for (int x = 0; x < col; x++)
			{
				if (!inventorySlots[y, x].IsLock)
				{
					AddCanUnlockSlot(x, y);
				}
			}
		}
	}
	
	private void AddCanUnlockSlot(int x,int y)
	{
		var inventorySlots = Inventory.Instance.InventorySlots;
        
		for (int i = 0; i < 4; i++)
		{
			int nx = x + Define.DIR_X[i];
			int ny = y + Define.DIR_Y[i];
			
			if (nx < 0 || ny < 0 || nx >= col || ny >= row) continue;

			if (inventorySlots[ny, nx].IsLock)
			{
				canUnlockSlotList.Add(inventorySlots[ny, nx]);
			}
		}
	}

	public int GetExistLockSlotCount()
	{
		foreach (var s in randomUnlockSlots)
			s.DisableFX();
		
		randomUnlockSlots.Clear();
		randomUnlockSlots.AddRange(canUnlockSlotList);

		if (randomUnlockSlots.Count > 0)
		{
			GetRandomUnlockableSlot();
		}

		return randomUnlockSlots.Count;
	}
    
	/// <summary>
	/// 해금 가능한 슬롯들 중 n개 랜덤으로 가져옴
	/// </summary>
	private void GetRandomUnlockableSlot()
	{
		int cnt = randomUnlockSlots.Count - unLockcnt;
		
		// 선택 가능한 슬롯 개수 이상이면 맞춰질때까지 삭제함
		while (cnt > 0)
		{
			randomUnlockSlots.RemoveAt(Random.Range(0, randomUnlockSlots.Count));
			cnt--;
		}
		//선택된 슬롯 테두리 켜줌
		foreach (var slot in randomUnlockSlots)
		{
			slot.EnableFX();
		}
	}

	public void RemoveUnLockSlot(InventorySlot slot)
	{
		canUnlockSlotList.Remove(slot);
		AddCanUnlockSlot(slot.Grid.X,slot.Grid.Y);
		
		foreach (var s in randomUnlockSlots)
		{
			s.DisableFX();
		}
	}
}