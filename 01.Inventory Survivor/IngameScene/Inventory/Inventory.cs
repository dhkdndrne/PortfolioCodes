using System;
using System.Collections.Generic;
using Bam.Extensions;
using UniRx;

using UnityEngine;
using VHierarchy.Libs;
using static Define;

public class Inventory : Bam.Singleton.Singleton<Inventory>
{
	[SerializeField] private InventorySetting inventorySetting;
	private float size = 0.5f;
	
	[Header("슬롯 프리팹")]
	[SerializeField] private GameObject slotPrefab;
	private InventorySlot[,] inventorySlots;

	[SerializeField] private Grid curGrid;
	private List<Grid> ghostGrids;

	private TempItemData tempItemData;
	private ColorManager colorManager;

	private Subject<Grid> onMouseSlotSubject;
	private bool canInsertItem;
	private SlotUnlockSystem slotUnlockSystem;
	private InventoryUnEquipedItemHolder inventoryUnEquipedItemHolder;
	public int rerollCost;
	
	public ReactiveProperty<int> UnLockCnt { get; private set; }
	public Grid CurGrid => curGrid;
	public TempItemData TempItemData => tempItemData;
	public InventorySlot[,] InventorySlots => inventorySlots;
	public InventoryUnEquipedItemHolder InventoryUnEquipedItemHolder => inventoryUnEquipedItemHolder;
	private void Start()
	{
		Init();
		InstantiateSlot();

		slotUnlockSystem.GetAllUnlockableSlot();
	}
	private void Init()
	{
		ghostGrids = new List<Grid>();
		inventoryUnEquipedItemHolder = GetComponent<InventoryUnEquipedItemHolder>();
		colorManager = ColorManager.Instance;
		tempItemData = new TempItemData();
		onMouseSlotSubject = new Subject<Grid>();
		UnLockCnt = new ReactiveProperty<int>();
		slotUnlockSystem = new SlotUnlockSystem();
		slotUnlockSystem.Init(inventorySetting);
		
		onMouseSlotSubject.Subscribe(grid =>
		{
			GetSlot(grid);
		}).AddTo(this);

		GameManager.Instance.Step.Where(step => step is GameStep.UnLockSlot).Subscribe(_ =>
		{
			int levelUpCnt = PlayerData.Instance.GetLevelUpCnt();
			if (levelUpCnt == 0)
				return;
			
			var slotCnt = slotUnlockSystem.GetExistLockSlotCount();
			UnLockCnt.Value = slotCnt < inventorySetting.UnlockSlotPerLevelUp ? slotCnt : levelUpCnt * inventorySetting.UnlockSlotPerLevelUp;
	
		}).AddTo(this);

		rerollCost = 10;
	}
	
	private void InstantiateSlot()
	{
		int col = inventorySetting.Col;
		int row = inventorySetting.Row;

		inventorySlots = new InventorySlot[row, col];
		var lockGrid = inventorySetting.InventoryLockGrid.Grids;

		for (int y = 0; y < row; y++)
		{
			for (int x = 0; x < col; x++)
			{
				var obj = Instantiate(slotPrefab, transform);
				obj.transform.InitLocalTransform(new Vector3(x * (size + 0.013f), 0, y * (size + 0.013f)), Quaternion.Euler(90, 0, 0), size);

				inventorySlots[y, x] = obj.GetComponent<InventorySlot>();
				inventorySlots[y, x].Init(x, y, onMouseSlotSubject, lockGrid[y, x] == 0);
			}
		}
	}

	public void GetSlot(Grid grid)
	{
		var dragItem = ItemDragHandler.Instance.DragItem;

		//아이템 드래그 중일때
		if (dragItem != null)
		{
			//이전 리스트 색깔 원래 대로 돌린 후 리스트 삭제
			ChangeGhostGridColor(colorManager.InvenColor.OnInitalColor);
			ghostGrids.Clear();

			if (grid != null)
			{
				canInsertItem = CheckValiable(dragItem, grid);
				ChangeGhostGridColor(canInsertItem ? colorManager.InvenColor.OnHoverEmptyCell : colorManager.InvenColor.OnHoverItemOverlapError);
			}
		}
		else // 아이템 드래그 중 아닐때
		{
			try
			{
				bool isNull = grid == null;
				var slot = isNull ? inventorySlots[curGrid.Y, curGrid.X] : inventorySlots[grid.Y, grid.X];

				if (slot.Item == null)
				{
					slot.ChangeColor(!isNull ? colorManager.InvenColor.OnHoverItem : colorManager.InvenColor.OnInitalColor);
				}
			}
			catch (Exception e)
			{
				Debug.Log(e);
			}
		}

		curGrid = grid;
	}

	private bool CheckValiable(InventoryItem dragItem, Grid grid)
	{
		int halfCol = (int)(ITEM_GRID_MAX_COL * 0.5f);
		int halfRow = (int)(ITEM_GRID_MAX_ROW * 0.5f);

		int startX = grid.X - halfCol;
		int startY = grid.Y - halfRow;

		bool check = true;

		for (int y = startY; y <= grid.Y + halfRow; y++)
		{
			for (int x = startX; x <= grid.X + halfCol; x++)
			{
				if (dragItem.ItemGrid[y - startY,x - startX] == 1)
				{
					bool isValid = CheckValidRange(x, y) && inventorySlots[y, x].Item == null;

					check &= isValid;

					if (isValid)
						ghostGrids.Add(inventorySlots[y, x].Grid);
				}
			}
		}

		return check;
	}

	public (bool, Grid) TryAddToInventory(InventoryItem item)
	{
		if (!canInsertItem || ghostGrids.Count == 0)
		{
			ChangeGhostGridColor(colorManager.InvenColor.OnInitalColor);
			ghostGrids.Clear();
			return (false, null);
		}

		foreach (var grid in ghostGrids)
		{
			AddToInventory(grid, item);
			item.ItemSlotGridList.Add(grid);
		}
		
		// 장착 아이템 리스트에 등록
		ItemManager.Instance.EquipItem(item);
	
		ghostGrids.Clear();
		return (true, curGrid);
	}
	
	public void UnEquipItem(InventoryItem item)
	{
		item.RemoveGridData();
		item.EnableCollider();
		
		if (item.ItemSo.ItemType is ItemType.Weapon)
			SynergyManager.Instance.RemoveSynergy(item.ItemSo as AttackItemSo);
		else
		{
			(item as InventoryAccessory).ApplyItemAbility(true);
		}
		ItemManager.Instance.UnEquipItem(item);
	}
	
	public void AddToInventory(Grid grid, InventoryItem item)
	{
		inventorySlots[grid.Y, grid.X].SetItem(item);
	}

	private bool CheckValidRange(int x, int y)
	{
		if (x < 0 || y < 0 || x >= inventorySetting.Col || y >= inventorySetting.Row) return false;
		if (inventorySlots[y, x].IsLock) return false;

		return true;
	}

	private void ChangeGhostGridColor(Color color)
	{
		foreach (var g in ghostGrids)
			inventorySlots[g.Y, g.X].ChangeColor(color);
	}

	public Vector3 GetSlotPosition(Grid grid)
	{
		return inventorySlots[grid.Y, grid.X].transform.position;
	}

	public void SaveBeforeData(InventoryItem item)
	{
		tempItemData.SaveData(item);
	}

	public void UnLockSlot(Grid grid)
	{
		UnLockCnt.Value--;
		slotUnlockSystem.RemoveUnLockSlot(inventorySlots[grid.Y, grid.X]);

		if (UnLockCnt.Value > 0)
		{
			slotUnlockSystem.GetExistLockSlotCount();
		}
	}

	public void GetNeighborItems(InventoryItem item,Grid grid)
	{
		int halfCol = (int)(ITEM_GRID_MAX_COL * 0.5f);
		int halfRow = (int)(ITEM_GRID_MAX_ROW * 0.5f);

		int startX = grid.X - halfCol;
		int startY = grid.Y - halfRow;

		var neighbor = item.NeighborItem;
        
		for (int y = startY; y <= grid.Y + halfRow; y++)
		{
			for (int x = startX; x <= grid.X + halfCol; x++)
			{
				if (CheckValidRange(x, y) && item.ItemGrid[y - startY, x - startX] == 1)
				{
					AddValidNeighbors(item, x, y, neighbor);
				}
			}
		}
		
		ItemManager.Instance.CheckNeighborBuffSkill(item);

		//이웃들에게 item을 이웃으로 추가
		foreach (var n in neighbor)
		{
			n.NeighborItem.Add(item);
			ItemManager.Instance.CheckNeighborBuffSkill(n);
		}
	}
	
	private void AddValidNeighbors(InventoryItem item, int x, int y, HashSet<InventoryItem> neighbor)
	{
		for (int i = 0; i < 4; i++)
		{
			int nx = x + DIR_X[i];
			int ny = y + DIR_Y[i];

			if (CheckValidRange(nx, ny) && inventorySlots[ny, nx].Item != null && inventorySlots[ny, nx].Item != item)
			{
				neighbor.Add(inventorySlots[ny, nx].Item);
			}
		}
	}

	public void ReollUnlockSlots()
	{
		if (GameManager.Instance.Step.Value is GameStep.UnLockSlot && PlayerData.Instance.Gold.Value >= rerollCost)
		{
			slotUnlockSystem.GetExistLockSlotCount();
			PlayerData.Instance.Gold.Value -= rerollCost;
			rerollCost += 10;
		}
	}
}

public class TempItemData
{
	public int AngleY { get; private set; }
	public List<Grid> GridList { get; private set; } = new List<Grid>();
	public int[,] ItemGrid { get; private set; }
	public Vector3 OriginPos { get; private set; }

	public void SaveData(InventoryItem item)
	{
		GridList.Clear();
		GridList.AddRange(item.ItemSlotGridList);
		AngleY = item.AngleY;
		ItemGrid = item.ItemGrid;
		OriginPos = item.OriginPos;
	}
}