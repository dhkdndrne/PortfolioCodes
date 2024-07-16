using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static UtilClass;
using Random = UnityEngine.Random;

public class ItemInfoUI : MonoBehaviour
{
	[Header("공용")]
	[SerializeField] private Image itemIcon;
	[SerializeField] private TextMeshProUGUI itemName;
	[SerializeField] private TextMeshProUGUI itemRank;
	[SerializeField] private TextMeshProUGUI priceText;

	[Header("무기")]
	[SerializeField] private GameObject weaponInfoPanel;
	[SerializeField] private TextMeshProUGUI synergy;
	[SerializeField] private TextMeshProUGUI abilityText;
	[SerializeField] private TextMeshProUGUI weaponSkillDescription;

	[Header("장신구 & 방어구")]
	[SerializeField] private TextMeshProUGUI otherAbilityText;
	[SerializeField] private TextMeshProUGUI otherSkillDescription;

	[Header("패널 하단 오브젝트")]
	[SerializeField] private GameObject lockImage;
	[SerializeField] private GameObject inventoryBtnRoot;


	private Image panel;
	private RectTransform rt;
	private float halfWidth;
	private float halfHeight;
	private InventoryItem curItem;
	private StringBuilder sb = new StringBuilder();
	private ColorManager colorManager;

	private void Awake()
	{
		rt = GetComponent<RectTransform>();
		halfWidth = rt.sizeDelta.x / 2;
		halfHeight = rt.sizeDelta.y / 2;
		panel = GetComponent<Image>();
		colorManager = ColorManager.Instance;

		Button sellBtn = inventoryBtnRoot.transform.GetChild(0).GetComponent<Button>();
		sellBtn.onClick.AddListener(() =>
		{
			if (curItem != null)
			{
				curItem.SellItem();
				gameObject.SetActive(false);
			}
		});

		Button pullOutBtn = inventoryBtnRoot.transform.GetChild(1).GetComponent<Button>();
		pullOutBtn.onClick.AddListener(() =>
		{
			curItem.transform.position = Inventory.Instance.InventoryUnEquipedItemHolder.GetSpawnPoint(Random.Range(0, 4)).position;
			Inventory.Instance.UnEquipItem(curItem);
			gameObject.SetActive(false);
		});
	}

	public void SetPosition(float size)
	{
		gameObject.SetActive(true);

		rt.sizeDelta = new Vector2(rt.sizeDelta.x, size);
		rt.position = Input.mousePosition + new Vector3(halfWidth, 0);
	}

	public void ShowShopItemInfo(ItemSo itemSo)
	{
		inventoryBtnRoot.SetActive(false);
		lockImage.SetActive(true);
		panel.raycastTarget = false;

		UpdateCommonUI(null,itemSo);
	}
	public void ShowInventoryItemInfo(InventoryItem item)
	{
		inventoryBtnRoot.SetActive(true);
		lockImage.SetActive(false);
		panel.raycastTarget = true;

		curItem = item;
		UpdateCommonUI(item,item.ItemSo);

		if (item.HasBuffSkill())
		{
			sb.Clear();
			sb.Append(abilityText.text);

			var buffSkill = item.Skill as BuffSkill;


			foreach (var t in buffSkill.GetAppliedBuffText())
				sb.Append(t).AppendLine();


			abilityText.text = sb.ToString();
		}

		priceText.text = $"판매(<color=#EFE61A>+{item.ItemSo.Price}g</color>)";
	}

	/// <summary>
	/// 상점 인벤토리 공용 UI 업데이트
	/// </summary>
	/// <param name="itemSo"></param>
	private void UpdateCommonUI(InventoryItem item, ItemSo itemSo)
	{
		itemIcon.sprite = itemSo.Sprite;
		itemName.text = itemSo.ItemName;

		itemRank.text = GetItemRankToString(itemSo.Rarity);
		itemRank.color = colorManager.GetItemRarityColor(itemSo.Rarity);

		weaponInfoPanel.SetActive(false);
		otherAbilityText.gameObject.SetActive(false);

		if (itemSo.ItemType == ItemType.Weapon)
		{
			var weaponSo = itemSo as AttackItemSo;
			weaponInfoPanel.SetActive(true);

			if (item == null)
			{
				sb.Clear();
				GetItemInfoToString(AbilityType.Damage, weaponSo.Damage, weaponSo.GetApplyAbilityDamage(0), false);         // 데미지
				sb.Append($"데미지 반영률: {weaponSo.ApplyAbilityValue * 100}%").AppendLine();                                    // 데미지 반영률
				GetItemInfoToString(AbilityType.CriChance, weaponSo.CriChance, weaponSo.GetApplyAbilityCriChance(0), true); // 크리확률
				GetItemInfoToString(AbilityType.CriPower, weaponSo.CriPower, weaponSo.GetApplyAbilityCriPower(0), true);    // 크리 데미지
				GetItemInfoToString(AbilityType.AtkRange, weaponSo.Range, weaponSo.GetApplyAbilityRange(0), false);         // 공격 범위
				GetItemInfoToString(AbilityType.AtkSpeed, weaponSo.AtkSpeed, weaponSo.GetApplyAbilityAtkSpeed(0), false);   // 공격 속도
			}
			else
			{
				var weaponItem = item as InventoryWeapon;

				sb.Clear();
				GetItemInfoToString(AbilityType.Damage, weaponSo.Damage, weaponSo.GetApplyAbilityDamage(weaponItem.GetBounusStat(AbilityType.Damage),true), false); // 데미지

				sb.Append($"데미지 반영률: {weaponSo.ApplyAbilityValue * 100}%").AppendLine();                                                                                  // 데미지 반영률
				GetItemInfoToString(AbilityType.CriChance, weaponSo.CriChance, weaponSo.GetApplyAbilityCriChance(weaponItem.GetBounusStat(AbilityType.CriChance)), true); // 크리확률
				GetItemInfoToString(AbilityType.CriPower, weaponSo.CriPower, weaponSo.GetApplyAbilityCriPower(weaponItem.GetBounusStat(AbilityType.CriPower)), true);     // 크리 데미지
				GetItemInfoToString(AbilityType.AtkRange, weaponSo.Range, weaponSo.GetApplyAbilityRange(weaponItem.GetBounusStat(AbilityType.AtkRange)), false);          // 공격 범위
				GetItemInfoToString(AbilityType.AtkSpeed, weaponSo.AtkSpeed, weaponSo.GetApplyAbilityAtkSpeed(weaponItem.GetBounusStat(AbilityType.AtkSpeed)), false);    // 공격 속도
			}

			abilityText.text = sb.ToString();
			sb.Clear();

			//시너지
			foreach (var id in weaponSo.synergyIdList)
			{
				sb.Append(SynergyManager.Instance.GetSynergy(id).Name).Append(" / ");
			}

			sb = sb.Remove(sb.Length - 3, 3);
			synergy.text = sb.ToString();
			weaponSkillDescription.text = itemSo.SkillDescription;
		}
		else
		{
			otherAbilityText.gameObject.SetActive(true);
			var nonAttackItem = itemSo as UnattackableItemSo;

			sb.Clear();

			foreach (var token in nonAttackItem.GetAbility())
			{
				if (token.value < 0)
					sb.Append(colorManager.GetColorString(token.value.ToString(), StringColor.Red));
				else
					sb.Append(colorManager.GetColorString("+" + token.value, StringColor.Green));

				sb.Append(" ");
				sb.Append(AbilityTypeToString(token.ability));
				sb.AppendLine();
			}
			otherAbilityText.text = sb.ToString();
			otherSkillDescription.text = itemSo.SkillDescription;
		}
	}

	private void GetItemInfoToString(AbilityType abilityType, float originValue, float changedValue, bool isPercent)
	{
		if (isPercent)
		{
			sb.Append(AbilityTypeToString(abilityType)).Append(": ").Append(colorManager.GetColorAbilityValue(abilityType, originValue, changedValue)).Append(" / ").Append(originValue).Append("%");
		}
		else
		{
			sb.Append(AbilityTypeToString(abilityType)).Append(": ").Append(colorManager.GetColorAbilityValue(abilityType, originValue, changedValue)).Append(" / ").Append(originValue);
		}

		sb.AppendLine();
	}
}