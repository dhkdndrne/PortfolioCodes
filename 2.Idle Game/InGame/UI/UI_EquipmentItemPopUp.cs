using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_EquipmentItemPopUp : MonoBehaviour
{
	#region Inspector

	[SerializeField] private TextMeshProUGUI titleTxt, itemNameTxt, itemRankTxt, ownEffectTxt, equipEffectTxt, levelTxt, pieceTxt;
	[SerializeField] private Image itemBG, itemIcon;
	[SerializeField] private Slider slider;
	[SerializeField] private Transform slotContainer;

	[SerializeField] private Button equipBtn, enhanceBtn;

    #endregion


	#region field

	private ReactiveProperty<Item_Equipment> curItem = new();
	private List<UI_EquipmentItemSlot> itemSlots;
	private List<(int ID, int Rank)> weaponKeyList;
	private List<(int ID, int Rank)> armorKeyList;

	private Action<ItemBase> onClickSlotAction;
	private StringBuilder sb;
    #endregion

	private void Awake()
	{
		Init();

		enhanceBtn.interactable = false;
		enhanceBtn.image.material = ImageManager.Instance.GrayScaleMaterial;
	}

	private void Init()
	{
		var prefab = slotContainer.GetChild(0).gameObject;

		onClickSlotAction += ClickSlot;

		itemSlots = new List<UI_EquipmentItemSlot>();

		weaponKeyList = ItemManager.Instance.GetItemIDList(ItemType.Equipment,EquipmentType.Weapon);
		armorKeyList = ItemManager.Instance.GetItemIDList(ItemType.Equipment,EquipmentType.Armor);
        
		foreach (var weapon in weaponKeyList)
		{
			var obj = Instantiate(prefab, slotContainer);
			var slot = obj.GetComponent<UI_EquipmentItemSlot>();

			slot.InitSlotUI(ItemManager.Instance.GetEquipmentData(weapon.Rank, weapon.ID), onClickSlotAction);
			itemSlots.Add(slot);
		}

		curItem.Subscribe(item =>
		{
			item?.Level.Subscribe(value =>
			{
				levelTxt.text = $"LV {value}";
				
				sb.Clear();
				sb.Append(item.OwnEffect.EffectType.GetOwnEffectTypeToString());
				sb.Append($" <color=#30BF3C>+{NumberTranslater.TranslateNumber(item.OwnEffect.Effect)}%</color>");
				ownEffectTxt.text = sb.ToString();

				sb.Clear();
				sb.Append(item.OwnEffect.EffectType.GetOwnEffectTypeToString());
				sb.Append($" <color=#30BF3C>+{NumberTranslater.TranslateNumber(item.EquippedEffect)}%</color>");
				equipEffectTxt.text = sb.ToString();
				
			}).AddTo(this);

			item?.piece.Subscribe(value =>
			{
				pieceTxt.text = $"{value} / {item.MaxPiece}";
				slider.value = (float)value / item.MaxPiece;
				
				bool check = value >= item.MaxPiece;
				enhanceBtn.interactable = check;
				enhanceBtn.image.material =  check ? null : ImageManager.Instance.GrayScaleMaterial;
				
			}).AddTo(this);
            
		}).AddTo(this);
		
		
		equipBtn.OnClickAsObservable().Where(_ => curItem != null).Subscribe(_ =>
		{
			Player.Instance.SetEquipment(curItem.Value);
			equipBtn.interactable = false;
			equipBtn.image.material = ImageManager.Instance.GrayScaleMaterial;

		}).AddTo(this);
		
		enhanceBtn.OnClickAsObservable().Where(_ => curItem != null).Subscribe(_ =>
		{
			curItem.Value.LevelUp();
			
			DataManager.Instance.RefreshItemData(ItemType.Equipment,curItem.Value);
            DataManager.Instance.SaveData();
            
		}).AddTo(this);
		sb = new StringBuilder();

		Destroy(prefab);
	}

	private void OpenEquipmentPopup(EquipmentType type)
	{
		gameObject.SetActive(true);
		bool isWeapon = type == EquipmentType.Weapon;

		titleTxt.text = isWeapon ? "무기" : "방어구";
		ClickSlot(Player.Instance.GetEquipment(type));

		int index = 0;

		ItemManager itemManager = ItemManager.Instance;
		List<(int ID, int Rank)> tempList = isWeapon ? weaponKeyList : armorKeyList;
		
		foreach (var slot in itemSlots)
		{
			slot.ChangeItem(itemManager.GetEquipmentData(tempList[index].Rank, tempList[index].ID));
			index++;
		}
	}

	public void OpenWeaponPopup()
	{
		OpenEquipmentPopup(EquipmentType.Weapon);
	}

	public void OpenArmorPopup()
	{
		OpenEquipmentPopup(EquipmentType.Armor);
	}

	private void ClickSlot(ItemBase item)
	{
		var equipment = item as Item_Equipment;
		Change(equipment);

		if (curItem.Value == equipment)
			return;

		curItem.Value = equipment;
		var itemVal = curItem.Value;
	
		itemNameTxt.text = itemVal.ItemName;
		itemRankTxt.text = itemVal.Rank.GetRankTypeToString();
        
		itemBG.sprite = ImageManager.Instance.GetItemRankBg(itemVal.Rank);
		itemIcon.sprite = ImageManager.Instance.GetItemIcon(ItemType.Equipment,itemVal.ID);
	}

	private void Change(Item_Equipment item)
	{
		bool isLock = item.IsLock.Value || item == Player.Instance.GetEquipment(item.Equipment_Type);

		equipBtn.interactable = !isLock;
		equipBtn.image.material = isLock ? ImageManager.Instance.GrayScaleMaterial : null;
	}

}