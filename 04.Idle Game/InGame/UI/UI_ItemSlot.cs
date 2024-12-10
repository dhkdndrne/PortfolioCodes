using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UI_ItemSlot : MonoBehaviour
{
	[SerializeField] protected Slider slider;
	[SerializeField] protected TextMeshProUGUI levelText, amountText;
	[SerializeField] protected Image itemIcon;
	[SerializeField] protected Image slotBG, lockIcon;

	public abstract void InitSlotUI(ItemBase item,Action<ItemBase> action);
}
