using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillPanel : MonoBehaviour
{
	[SerializeField] private Button[] equippedSkillBtns;
	[SerializeField] private Transform slotContainer;
	[SerializeField] private UI_SkillPopUp skillInfoPopUp;
    
	private Action<ItemBase> onClickSlotAction;

	private void Awake()
	{
		var prefab = slotContainer.GetChild(0).gameObject;

		var itemManager = ItemManager.Instance;
		var playerData = DataManager.Instance.PlayerData;

		onClickSlotAction += skillInfoPopUp.OpenPopUp;

		var list = itemManager.GetItemIDList(ItemType.Skill);

		foreach (var skill in list)
		{
			var obj = Instantiate(prefab, slotContainer);
			var slot = obj.GetComponent<UI_SkillItemSlot>();

			slot.InitSlotUI(itemManager.GetSkillData(skill.Rank,skill.ID), onClickSlotAction);
		}

		Destroy(prefab);

		var skillSystem = Player.Instance.SkillSystem;

		//장착 스킬 아이콘 초기화
		for (int i = 0; i < equippedSkillBtns.Length; i++)
		{
			int index = i;

			int id = playerData.equippedSkillIDs[index].id;
			ChangeEquippedSkillSlot(id, index);

			equippedSkillBtns[index].onClick.AddListener(() =>
			{
				var skillItem = skillSystem.GetSkillItem(index);

				if (skillItem == null)
					return;

				onClickSlotAction?.Invoke(skillItem);
			});
		}

		skillSystem.onEquipSkillAction += ChangeEquippedSkillSlot;
	}

	private void ChangeEquippedSkillSlot(int id, int index)
	{
		equippedSkillBtns[index].image.sprite = id == 0 ? ImageManager.Instance.EquippedSkillSlotBaseSprite : ImageManager.Instance.GetItemIcon(ItemType.Skill,id);
	}
}