using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ItemSo : ScriptableObject
{
	[SerializeField] private ItemGrid itemGrid;

	[Header("정보")]
	[SerializeField] private int id;
	[SerializeField] protected string itemName;
	[SerializeField] private ItemType itemType;

	[SerializeField] private Sprite sprite;

	[SerializeField] private InventoryItem inventoryItemPrefab; // 상점 및 인벤토리에서 쓸 프리팹
	[SerializeField] private Weapon weaponPrefab;               // 인게임에서 사용할 프리팹

	[SerializeField] protected ItemRarity rarity;
	[SerializeField] private Skill skill;

	protected int price;

	public int ID => id;
	public string ItemName => itemName;
	public int Price => price;
	public ItemType ItemType => itemType;
	public Sprite Sprite => sprite;
	public ItemRarity Rarity => rarity;
	public ItemGrid ItemGrid => itemGrid;
	public InventoryItem InventoryItem => inventoryItemPrefab;
	public GameObject WeaponPrefab => weaponPrefab.gameObject;
	public string SkillDescription => skill?.Description;
	public Skill Skill => skill;
	public Skill GetSkillInstance()
	{
		if (skill == null)
			return null;
		
		return Instantiate(skill);
	}
}

[Serializable]
public class ItemGrid
{
	[SerializeField] private bool isInit;
	[SerializeField] private List<int> gridList = new List<int>();

	private int[,] grids;

	public List<int> GridList
	{
		get { return gridList; }
		set { gridList = value; }
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

	public void Init()
	{
		if (isInit && gridList.Count == ITEM_GRID_MAX_COL * ITEM_GRID_MAX_ROW)
			return;

		int size = ITEM_GRID_MAX_COL * ITEM_GRID_MAX_ROW;
		gridList.Clear();

		for (int i = 0; i < size; i++)
			gridList.Add(0);

		Debug.Log("ItemGrid 초기화");
		isInit = true;
	}

	private void ListToArray()
	{
		grids = new int[ITEM_GRID_MAX_ROW, ITEM_GRID_MAX_COL];

		for (int y = 0; y < ITEM_GRID_MAX_ROW; y++)
		{
			for (int x = 0; x < ITEM_GRID_MAX_COL; x++)
			{
				//인스펙터에서 지정한 모양대로 나오게 하기
				int revisedRow = (ITEM_GRID_MAX_ROW - 1) - y;
				grids[y, x] = gridList[revisedRow * ITEM_GRID_MAX_COL + x];
			}
		}
	}
	
}