using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class UI_StatUpgrade : MonoBehaviour
{
    #region Inspector

	[SerializeField] private Transform slotContainer;
	[SerializeField] private GameObject slotPrefab;

    #endregion

	private List<UI_StatUpgradeSlot> slotList;

	private void Awake()
	{
		slotList = slotContainer.GetComponentsInChildren<UI_StatUpgradeSlot>().ToList();
		Initializator.Instance.onSecondInit += Init;
	}

	private void Init()
	{
		var enumValue = Enum.GetValues(typeof(StatType));

		int gap = enumValue.Length - slotList.Count;

		//스탯 enum의 개수가 미리 생성해놓은 슬롯 보다 많으면 차이만큼 슬롯 생성
		while (gap > 0)
		{
			var slot = Instantiate(slotPrefab, slotContainer);
			slotList.Add(slot.GetComponent<UI_StatUpgradeSlot>());
			gap--;
		}

		Player player = Player.Instance;

		for (int i = 0; i < enumValue.Length; i++)
		{
			var stat = (StatType)i;

			slotList[i].Icon.sprite = ImageManager.Instance.GetBaseStatIcon(stat);
			slotList[i].StatNameText.text = stat switch
			{
				StatType.Damage => "데미지",
				StatType.Heal => "체력 회복",
				StatType.Health => "체력",
				StatType.CriRate => "치명타 확률",
				StatType.CriDamge => "치명타 피해",
				StatType.AtkSpeed => "공격속도",
				_ => "새로운 스텟추가"
			};

			bool isPercent = (StatType)i is StatType.AtkSpeed or StatType.CriRate or StatType.CriDamge;

			//슬롯 UI갱신 추가
			int index = i;
			player.StatDic[stat].level.Subscribe(value =>
			{
				slotList[index].LevelText.text = $"Lv {value}";
			}).AddTo(gameObject);

			player.StatDic[stat].effect.Subscribe(value =>
			{
				slotList[index].ValueText.text = isPercent ? $"{value.TranslateNumber()}%" : value.TranslateNumber();
			}).AddTo(gameObject);

			player.StatDic[stat].upgradeCost.Subscribe(value =>
			{
				slotList[index].UpgradeCostText.text = NumberTranslater.TranslateNumber(value);
			}).AddTo(gameObject);

			//최초 갱신
			slotList[index].LevelText.text = $"Lv {player.StatDic[stat].level.Value}";
			slotList[index].ValueText.text = isPercent ? $"{Math.Truncate(player.StatDic[stat].effect.Value)}%" : Math.Truncate(player.StatDic[stat].effect.Value).ToString();
			slotList[index].UpgradeCostText.text = NumberTranslater.TranslateNumber(player.StatDic[stat].upgradeCost.Value);
            
			//버튼
			slotList[index].UpgradeBtn.GetComponent<LongTouchButton>().Init(0.01f, 0.5f, player.StatDic[stat].Upgrade);
		}
	}
}