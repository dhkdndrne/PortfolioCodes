using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_Battle : MonoBehaviour
{
	[SerializeField] private Slider stageSlider;
	[SerializeField] private TextMeshProUGUI stageNameText;

	[Header("메인 화면 장착 스킬 버튼")]
	[SerializeField] private Button[] equippedSkillBtns;

	[Header("자동스킬 버튼")]
	[SerializeField] private Button autoSkillBtn;

	[Header("스테이지 무한반복 로고")]
	[SerializeField] private GameObject infinityStageLogo;

	private Image[] coolTimeImages;
	private Image[] durationImages;
	private SkillSystem skillSystem;

	public void ToggleAutoSkill()
	{
		Player.Instance.SkillSystem.IsAutoMode = !Player.Instance.SkillSystem.IsAutoMode;
		autoSkillBtn.transform.GetChild(0).gameObject.SetActive(Player.Instance.SkillSystem.IsAutoMode);
	}

	private readonly float[] sliderValue = new[] { 0, 0.2f, 0.5f, 0.8f, 1f };
	private void Start()
	{
		StageManager.Instance.CurStage.Wave.Subscribe(wave =>
		{
			if (wave == 0)
				stageSlider.value = 0;
			else
				SliderAnimation(sliderValue[wave]).Forget();
		}).AddTo(this);

		var playerData = DataManager.Instance.PlayerData;
		skillSystem = Player.Instance.SkillSystem;

		coolTimeImages = new Image[equippedSkillBtns.Length];
		durationImages = new Image[equippedSkillBtns.Length];

		//장착 스킬 아이콘 초기화
		for (int i = 0; i < equippedSkillBtns.Length; i++)
		{
			int index = i;
			int id = playerData.equippedSkillIDs[index].id;
			ChangeEquippedSkillSlot(id, index);

			//장착 스킬 버튼 이벤트 추가
			equippedSkillBtns[index].onClick.AddListener(() =>
			{
				var skill = skillSystem.GetSkillItem(index);

				if (skill == null)
				{
					UtilClass.DebugLog("스킬 비었음");
				}
				else
				{
					if (skill.Skill.CanUse)
						skill.Skill.UseSkill().Forget();
				}
			});

			//쿨타임 이미지 세팅
			coolTimeImages[index] = equippedSkillBtns[index].transform.GetChild(0).GetComponent<Image>();
			durationImages[index] = equippedSkillBtns[index].transform.GetChild(1).GetComponent<Image>();
		}
		autoSkillBtn.transform.GetChild(0).gameObject.SetActive(DataManager.Instance.PlayerData.isAutoSkill);
		skillSystem.onEquipSkillAction += ChangeEquippedSkillSlot;

		StageManager.Instance.IsInfinityStage.Subscribe(value =>
		{
			infinityStageLogo.SetActive(value);
		}).AddTo(this);

		StageManager.Instance.CurStage.StageName.Subscribe(value =>
		{
			stageNameText.text = value;
		}).AddTo(this);
	}

	private void Update()
	{
		for (int i = 0; i < equippedSkillBtns.Length; i++)
		{
			var skill = skillSystem.GetSkillItem(i);

			if (skill is null)
				continue;

			if (!skill.Skill.IsUsing)
			{
				coolTimeImages[i].fillAmount = skill.Skill.GetCoolTimeVal();
			}
			else if (skill.Skill.IsUsing)
			{
				durationImages[i].fillAmount = skill.Skill.GetElapsedTime();
			}
		}
	}

	private async UniTaskVoid SliderAnimation(float targetValue)
	{
		await stageSlider.DOValue(targetValue, 0.3f);
	}

	private void ChangeEquippedSkillSlot(int id, int index)
	{
		equippedSkillBtns[index].image.sprite = id == 0 ? ImageManager.Instance.EquippedSkillSlotBaseSprite : ImageManager.Instance.GetItemIcon(ItemType.Skill,id);
	}
}