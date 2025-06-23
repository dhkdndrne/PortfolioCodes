using System;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_EquipmentItemSlot : UI_ItemSlot
{
	[SerializeField] private Button button;

	private ReactiveProperty<Item_Equipment> equipmentItem = new();

	public void ChangeItem(Item_Equipment item)
	{
		equipmentItem.Value = item;

		bool isLock = equipmentItem.Value.IsLock.Value;
		lockIcon.gameObject.SetActive(isLock);
		itemIcon.material = isLock ? ImageManager.Instance.GrayScaleMaterial : null;
		slotBG.material = isLock ? ImageManager.Instance.GrayScaleMaterial : null;
        
		slotBG.sprite = ImageManager.Instance.GetItemRankBg(equipmentItem.Value.Rank);
		itemIcon.sprite = ImageManager.Instance.GetItemIcon(ItemType.Equipment,equipmentItem.Value.ID);
	}
	public override void InitSlotUI(ItemBase item, Action<ItemBase> action)
	{
		ImageManager imageManager = ImageManager.Instance;

		equipmentItem.Value = item as Item_Equipment;
		button.onClick.AddListener(() =>
		{
			transform.DOScale(Vector3.one * 1.1f, 0.05f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InFlash);
			action.Invoke(equipmentItem.Value);
		});

		equipmentItem.Subscribe(item =>
		{
			item.Level.Subscribe(value =>
			{
				levelText.text = $"LV.{value}";
			}).AddTo(this);

			item.piece.Subscribe(value =>
			{
				amountText.text = $"{value} / {item.MaxPiece}";
				slider.value = (float)value / item.MaxPiece;
			}).AddTo(this);
            
		}).AddTo(this);
		
		slotBG.sprite = imageManager.GetItemRankBg(equipmentItem.Value.Rank);
		itemIcon.sprite = imageManager.GetItemIcon(ItemType.Equipment,equipmentItem.Value.ID);
	}
}