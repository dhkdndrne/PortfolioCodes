using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
	private enum FilterOption
	{
		All,
		Equipment,
		Consumable,
		Etc
	}

	[SerializeField] private GameObject slotPrefab;   // 슬롯 UI 프리팹
	[SerializeField] private Transform slotContainer; // 슬롯 UI를 담을 transform
	[SerializeField] private GhostItemUI ghostItem;
	[SerializeField] private Button sortButton;

	[Header("정렬 토글버튼")]
	[SerializeField] private Toggle toggle_All;
	[SerializeField] private Toggle toggle_Equipments;
	[SerializeField] private Toggle toggle_ConsumeItems;
	[SerializeField] private Toggle toggle_Etc;

	[SerializeField] private ItemPanelUI itemInfoPanel;
	[SerializeField] private DividePanelUI dividePanel;

	private Inventory inventory; // 인벤토리
	private SlotUI[] slots;      // 슬롯 UI들을 담을 배열

	private int toggleIndex; // 선택되어있는 토글버튼 index
	private int dragStartIndex;

	private FilterOption curFilterOption = FilterOption.All;

	private Action<int> mouseEnterAction;
	private Action<int> mouseClickAction;
	private Action<int> mouseDragEndAction;
	private Action<int, int, int> divideAction;

	private void Start()
	{
		Init();
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Mouse1) && !Input.GetMouseButton(0))
		{
			var slot = Utill.RaycastAndGetFirstComponent<SlotUI>();

			if (slot == null)
				return;

			int index = slot.Index;
			var item = inventory.GetInveoryItem(index);

			if (item == null)
				return;

			slot.SetSlotLock(inventory.LockSlot(index));
		}
	}
	
	private void Init()
	{
		InitToggleEvent();
		mouseEnterAction += SelectSlot;
		mouseClickAction += StartItemDrag;
		mouseDragEndAction += EndDrag;

		ghostItem.Init(mouseDragEndAction);
	}


	private void InitToggleEvent()
	{
		toggle_All.onValueChanged.AddListener(isOn => ChangeSlotFilter(isOn, FilterOption.All));
		toggle_Equipments.onValueChanged.AddListener(isOn => ChangeSlotFilter(isOn, FilterOption.Equipment));
		toggle_ConsumeItems.onValueChanged.AddListener(isOn => ChangeSlotFilter(isOn, FilterOption.Consumable));
		toggle_Etc.onValueChanged.AddListener(isOn => ChangeSlotFilter(isOn, FilterOption.Etc));
	}

	private void ChangeSlotFilter(bool isOn, FilterOption option)
	{
		//같은 토글 버튼 눌렀으면 return (중복 방지)
		if (curFilterOption == option)
			return;

		if (isOn)
		{
			// 토글의 옵션을 갱신해줌
			curFilterOption = option;

			//전체 슬롯을 돌면서 필터를 적용
			for (int i = 0; i < slots.Length; i++)
			{
				ApplyFilterToSlot(i, inventory.GetInveoryItem(i));
			}
		}
	}

	private void ApplyFilterToSlot(int index, Item item)
	{
		// item이 null이면 filter 타입이 all이 아닌이상 무조건 검정색으로 보여야하므로
		bool isApplied = curFilterOption is FilterOption.All;

		//null 이 아니면 filter에 맞는 타입의 아이템인지 체크
		if (item != null)
			isApplied = IsFilterAppliedSlot(item);

		slots[index].SetSlotAccessState(isApplied);
	}

	private bool IsFilterAppliedSlot(Item item)
	{
		return curFilterOption switch
		{
			FilterOption.Equipment => item.Data.ItemType is ItemType.Equipment,
			FilterOption.Consumable => item.Data.ItemType is ItemType.Consumable,
			FilterOption.Etc => item.Data.ItemType is ItemType.Etc,
			_ => true
		};
	}

	private void EndDrag(int targetIndex)
	{
		ghostItem.gameObject.SetActive(false); // 더미 이미지 비활성화

		// 같은 슬롯에 놓았거나, 슬롯이 아닌 위치에 놓았을때
		if (dragStartIndex == targetIndex
		    || targetIndex == -1
		    || !slots[targetIndex].IsAppliedFilter
		    || inventory.GetLockesSlot(targetIndex))
		{
			// 지웠던 슬롯 이미지를 다시 복구해준다.
			slots[dragStartIndex].UpdateSlotUI(inventory.GetInveoryItem(dragStartIndex));
			return;
		}

		var originSlot = inventory.GetInveoryItem(dragStartIndex);
		var targetSlot = inventory.GetInveoryItem(targetIndex);

		if (Input.GetKey(KeyCode.LeftControl)
		    && !inventory.GetLockesSlot(targetIndex)
		    && originSlot.Data.MaxAmount != 1
		    && (targetSlot == null || originSlot.Data.ID == targetSlot.Data.ID))
		{
			dividePanel.OpenPanel(dragStartIndex, targetIndex, inventory.GetInveoryItem(dragStartIndex).Amount);
			slots[dragStartIndex].UpdateSlotUI(inventory.GetInveoryItem(dragStartIndex));
		}
		else
		{
			// 인벤토리에서 두 슬롯의 아이템을 교체해준다.
			inventory.ChangeSlotData(dragStartIndex, targetIndex);
		}
	}

	/// <summary>
	/// 인벤토리 설정
	/// </summary>
	/// <param name="inventory"></param>
	public void SetInventory(Inventory inventory)
	{
		this.inventory = inventory;
		CreateSlotUI();

		divideAction += inventory.DivideItem;
		dividePanel.Init(divideAction);
		sortButton.onClick.AddListener(inventory.SortInventory);
	}

	/// <summary>
	/// 슬롯 UI 오브젝트 생성 후 넣어줌
	/// </summary>
	private void CreateSlotUI()
	{
		int capacity = inventory.CAPACITY;
		slots = new SlotUI[capacity];

		for (int i = 0; i < capacity; i++)
		{
			var obj = Instantiate(slotPrefab, slotContainer);
			slots[i] = obj.GetComponent<SlotUI>();

			int index = i;
			slots[i].Init(mouseEnterAction, mouseClickAction, index);
		}
	}

	/// <summary>
	/// 마우스 위치의 슬롯을 받아온다.
	/// </summary>
	/// <param name="index"></param>
	private void SelectSlot(int index)
	{
		var item = inventory.GetInveoryItem(index);

		if (item == null)
		{
			itemInfoPanel.gameObject.SetActive(false);
			return;
		}

		itemInfoPanel.gameObject.SetActive(true);
		itemInfoPanel.UpdateUI(item);
	}

	private void StartItemDrag(int index)
	{
		if (inventory.GetLockesSlot(index))
			return;

		var item = inventory.GetInveoryItem(index);

		dragStartIndex = index;
		ghostItem.ChangeUI(item);

		//클릭한 슬롯UI 지워준다.
		slots[index].UpdateSlotUI(null);
	}

	/// <summary>
	/// 슬롯 UI 업데이트
	/// </summary>
	/// <param name="list"></param>
	/// <param name="itemList"></param>
	public void UpdateSlotUI(List<int> list)
	{
		foreach (var index in list)
			slots[index].UpdateSlotUI(inventory.GetInveoryItem(index));
	}

	public void UpdateAllSlots()
	{
		for (int i = 0; i < slots.Length; i++)
		{
			var item = inventory.GetInveoryItem(i);
			slots[i].UpdateSlotUI(item);

			ApplyFilterToSlot(i, item);
		}
	}
}