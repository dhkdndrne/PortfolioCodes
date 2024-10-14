using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Inventory : MonoBehaviour
{
	[SerializeField] private InventoryUI inventoryUI;
	[SerializeField] private bool[] lockedSlots;
	[SerializeField] private ItemData[] test;

	private Item[] itemArray;
	private List<int> updateIndexList;
	public readonly int CAPACITY = 100;

	public Item GetInveoryItem(int index) => itemArray[index];
	public bool GetLockesSlot(int index) => lockedSlots[index];
	private void Start()
	{
		Init();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			var item = test[0];
			int amount = Random.Range(1, 101);
			AddItem(item, amount);
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			var item = test[1];
			int amount = Random.Range(1, 101);
			AddItem(item, amount);
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			var item = test[2];
			int amount = Random.Range(1, 10);
			AddItem(item, amount);
		}
		if (Input.GetKeyDown(KeyCode.F))
		{
			var item = test[3];
			int amount = Random.Range(1, 10);
			AddItem(item, amount);
		}
		if (Input.GetKeyDown(KeyCode.G))
		{
			var item = test[4];
			int amount = Random.Range(1, 100);
			AddItem(item, amount);
		}
	}

	private void Init()
	{
		itemArray = new Item[CAPACITY];
		lockedSlots = new bool[CAPACITY];
		inventoryUI.SetInventory(this);

		updateIndexList = new List<int>();
	}
    
	public bool LockSlot(int index)
	{
		lockedSlots[index] = !lockedSlots[index];
		return lockedSlots[index];
	}

	private void AddItem(ItemData itemData, int amount)
	{
		updateIndexList.Clear();

		// 여러개 들고있을 수 있는 아이템일때
		if (itemData.MaxAmount > 1)
		{
			AddStackableItem(itemData, amount);
		}
		else
		{
			int index = -1;
			int amt = amount; // 넣을 양

			while (amt > 0)
			{
				index = FindEmptySlot();

				//비어있는 슬롯이 없으면 탈출
				if (index == -1)
					break;

				itemArray[index] = new Item(itemData);
				amt = itemArray[index].TryAdd(amt);

				// ui업데이트 슬롯 index 추가
				updateIndexList.Add(index);
			}
		}

		// 슬롯 ui 업데이트할게 있으면 업데이트 해줌
		if (updateIndexList.Count > 0)
			inventoryUI.UpdateSlotUI(updateIndexList);
	}

	private void AddStackableItem(ItemData itemData, int amount)
	{
		// 같은 종류 아이템 찾기
		// 더할 수 있는 슬롯에 다 더함 -> 다 넣거나 더해줄 슬롯이 없을때 까지
		// 만약 더할 수 있는 슬롯이 없으면 새로운 슬롯에 넣음
		int amt = amount; // 넣을 양
		bool hasOtherSlot = true;
		int index = -1;

		while (amt > 0)
		{
			if (hasOtherSlot)
			{
				index = FindStackableItemSlot(itemData, index + 1);

				if (index == -1)
				{
					hasOtherSlot = false;
					continue;
				}

				amt = itemArray[index].TryAdd(amt);

				// ui업데이트 슬롯 index 추가
				updateIndexList.Add(index);
			}
			else
			{
				index = FindEmptySlot();

				//비어있는 슬롯이 없으면 탈출
				if (index == -1)
					break;

				itemArray[index] = new Item(itemData);
				amt = itemArray[index].TryAdd(amt);

				// ui업데이트 슬롯 index 추가
				updateIndexList.Add(index);
			}
		}
	}

	/// <summary>
	/// 비어있는 슬롯 인덱스 반환
	/// </summary>
	/// <param name="startIndex"></param>
	/// <returns></returns>
	private int FindEmptySlot(int startIndex = 0)
	{
		for (int i = startIndex; i < CAPACITY; i++)
		{
			if (IsEmptySlot(i) && !lockedSlots[i])
				return i;
		}

		return -1;
	}

	public void DivideItem(int originIndex,int targetIndex,int amount)
	{
		updateIndexList.Clear();

		if (itemArray[targetIndex] == null)
			itemArray[targetIndex] = new Item(itemArray[originIndex].Data);
        
		int curAmount = itemArray[targetIndex].TryAdd(amount);
		
		if (curAmount == 0)
		{
			itemArray[originIndex].Amount -= amount;

			if (itemArray[originIndex].Amount == 0)
				itemArray[originIndex] = null;
		}
		else
		{
			itemArray[originIndex].Amount = curAmount;
		}
		
		updateIndexList.Add(originIndex);
		updateIndexList.Add(targetIndex);
		
		inventoryUI.UpdateSlotUI(updateIndexList);
	}
	
	private bool IsEmptySlot(int index)
	{
		return itemArray[index] == null;
	}

	/// <summary>
	/// 해당 아이템이 이미 인벤토리에 존재하고 추가 가능한지 체크 후 인덱스 반환
	/// </summary>
	private int FindStackableItemSlot(ItemData itemData, int startIndex = 0)
	{
		int id = itemData.ID;
		int max = itemData.MaxAmount;

		for (int i = startIndex; i < CAPACITY; i++)
		{
			if (IsEmptySlot(i) || lockedSlots[i])
				continue;

			if (id == itemArray[i].Data.ID && itemArray[i].Amount < max)
				return i;

		}
		return -1;
	}

	public void ChangeSlotData(int startIndex, int targetIndex)
	{
		var originItem = itemArray[startIndex];
		var targetItem = itemArray[targetIndex];
		bool swap = true;

		if (targetItem != null)
		{
			bool canStack = originItem.Data.MaxAmount > 1 && targetItem.Data.MaxAmount > 1 && originItem.Data.ID == targetItem.Data.ID;

			// 더할 수 있고 같은 아이템일때
			if (canStack)
			{
				int amount = originItem.Amount;
				amount = targetItem.TryAdd(amount);
				swap = false;

				//전부 더해졌을때
				if (amount == 0)
				{
					itemArray[startIndex] = null;
				}
				else if (amount == originItem.Amount)
				{
					// 두 수가 같다는건 더해진게 없다는 뜻 즉,타겟 슬롯의 개수가 꽉참 그냥 교환
					swap = true;
				}
				else
				{
					//원래 슬롯의 개수에 더하고 남은 숫자를 대입
					originItem.Amount = amount;
				}
			}
		}

		if (swap)
		{
			//아니면 위치 교환
			(itemArray[startIndex], itemArray[targetIndex]) = (itemArray[targetIndex], itemArray[startIndex]);
		}

		//갱신이 필요한 슬롯의 인덱스들을 갱신 리스트에 넣음
		updateIndexList.Add(startIndex);
		updateIndexList.Add(targetIndex);

		// inventoryUI에게 갱신을 요청
		inventoryUI.UpdateSlotUI(updateIndexList);
	}

	public void SortInventory()
	{
		//인벤토리 전체 길이만큼
		for (int i = 0; i < CAPACITY; i++)
		{
			//슬롯에 아이템이 존재하고 잠긴 슬롯이 아닐때
			if (itemArray[i] != null && !lockedSlots[i])
			{
				//아이템의 최대개수
				int maxAmount = itemArray[i].Data.MaxAmount;
				//아이템의 최대개수가 1보다 크고 현재 아이템의 개수가 최대 개수보다 적을때
				if (maxAmount > 1 && itemArray[i].Amount < maxAmount)
				{
					//현재 슬롯의 아이템 데이터를 저장 후 인벤토리에서 임시 삭제
					var item = itemArray[i];
					itemArray[i] = null;
					//같은 아이템을 찾아 더하고,없으면 빈 슬롯에 넣어준다.
					AddStackableItem(item.Data, item.Amount);
				}
			}
		}

		//잠금 슬롯 빼고 나머지가 저장된다.
		var sortedItems = itemArray
			.Where((item, index) => !lockedSlots[index] && item != null)
			.OrderBy(item => item.Data.ItemType)
			.ThenByDescending(item => item.Data.Rank)
			.ThenBy(item => item.Data.ID)
			.ThenByDescending(item => item.Amount)
			.ToList();

		int index = 0;
		for (int i = 0; i < CAPACITY; i++)
		{
			if (lockedSlots[i])
				continue;

			itemArray[i] = index == sortedItems.Count ? null : sortedItems[index++];
		}

		inventoryUI.UpdateAllSlots();
	}
    
}