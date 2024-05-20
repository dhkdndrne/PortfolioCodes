using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

public class UI_CharacterPopup : MonoBehaviour
{
	[Header("보유효과 버튼")]
	[SerializeField] private Button[] ownEffectButtons;

	[Header("보유 효과 클릭 팝업")]
	[SerializeField] private RectTransform ownInfoPopup;
	[SerializeField] private TextMeshProUGUI ownInfoText;
	[SerializeField] private Vector2[] popUpPos;

	[SerializeField] private Transform slotContainer;

	[Header("버튼")]
	[SerializeField] private Button equipButton, enhanceButton;
	[SerializeField] private TextMeshProUGUI enhanceText;

	[SerializeField] private Image equippedCharIcon;
	[SerializeField] private TextMeshProUGUI equippedCharName, equippedCharRank;

	[Header("슬라이더")]
	[SerializeField] private Slider pieceSlider;
	[SerializeField] private TextMeshProUGUI levelText;
	[FormerlySerializedAs("amounText")] [SerializeField] private TextMeshProUGUI amountText;

	private StringBuilder sb;
	private int curEffectIndex;
	private Action<ItemBase> onClickSlotAction;
	private ReactiveProperty<Item_Character> curCharacter = new ReactiveProperty<Item_Character>();

	private void Awake()
	{
		Init();
	}

	public void OpenPopUp()
	{
		gameObject.SetActive(true);
		curCharacter.Value = Player.Instance.EquippedCharacter;
		ChangeUI(curCharacter.Value);
	}

	private void ChangeUI(Item_Character character)
	{
		curCharacter.Value = character;

		equippedCharIcon.sprite = ImageManager.Instance.GetCharacterIcon(character.ID);
		equippedCharName.text = character.ItemName;
		equippedCharRank.text = character.Rank.GetRankTypeToString();
        
		enhanceText.text = curCharacter.Value.IsLock.Value
			? $"구매\n {NumberTranslater.TranslateNumber(character.BuyCost)}"
			: $"강화\n {NumberTranslater.TranslateNumber(character.LevelUpBaseCost * (1 + character.LevelPerCostRate * character.Level.Value))}";
		ownInfoPopup.gameObject.SetActive(false);
	}

	private void Init()
	{
		sb = new StringBuilder();

		var prefab = slotContainer.GetChild(0).gameObject;
		onClickSlotAction += item => ChangeUI(ItemManager.Instance.GetCharacterDic()[item.ID]);
		var keyList = ItemManager.Instance.GetCharacterDic().Keys.ToList();

		for (int i = 0; i < keyList.Count; i++)
		{
			var obj = Instantiate(prefab, slotContainer);
			var slot = obj.GetComponent<UI_CharacterItemSlot>();

			slot.InitSlotUI(ItemManager.Instance.GetCharacterData(keyList[i]), onClickSlotAction);
		}

		Destroy(prefab);

		for (int i = 0; i < ownEffectButtons.Length; i++)
		{
			int index = i;
			ownEffectButtons[index].OnClickAsObservable().Subscribe(_ =>
			{
				if (index == curEffectIndex && ownInfoPopup.gameObject.activeSelf)
				{
					ownInfoPopup.gameObject.SetActive(false);
					return;
				}

				curEffectIndex = index;
				sb.Clear();
				ownInfoPopup.gameObject.SetActive(true);
				ownInfoPopup.anchoredPosition = popUpPos[index];

				var ownEffects = curCharacter.Value.OwnEffects;

				string effectType = ownEffects[index].EffectType.GetOwnEffectTypeToString();

				sb.Append(effectType).Append(" ");
				sb.Append($"{NumberTranslater.TranslateNumber(ownEffects[index].Effect)}% 증가");

				ownInfoText.text = sb.ToString();
			});
		}

		curCharacter.Subscribe(item =>
		{
			item?.Level.Subscribe(val =>
			{
				levelText.text = $"LV {val}";
			}).AddTo(this);

			item?.piece.Subscribe(val =>
			{
				amountText.text = $"{val} / {item.MaxPiece}";
				pieceSlider.value = (float)val / item.MaxPiece;
			}).AddTo(this);

			if (item != null)
			{
				var effects = item.OwnEffects;
				for (int i = 0; i < ownEffectButtons.Length; i++)
				{
					ownEffectButtons[i].image.sprite = ImageManager.Instance.GetOwnEffectTypeIcon(effects[i].EffectType);
				}
			}
		}).AddTo(this);
		
		enhanceButton.onClick.AddListener(() =>
		{
			bool check = false;
			var charValue = curCharacter.Value;
			BigInteger cost = 0;
			
			if (charValue.IsLock.Value)
			{
				cost = new BigInteger(charValue.BuyCost);
				if (cost <= Player.Instance.Currency.gold.Value)
				{
					Player.Instance.Currency.DecreaseGold(cost);
					charValue.IsLock.Value = false;
					check = true;
                    
				}
			}
			else
			{
				cost = new BigInteger(charValue.LevelUpBaseCost * (1 + charValue.LevelPerCostRate * charValue.Level.Value));
				if (cost <= Player.Instance.Currency.gold.Value)
				{
					Player.Instance.Currency.DecreaseGold(cost);
					charValue.piece.Value++;
					if (charValue.piece.Value == charValue.MaxPiece)
					{
						charValue.LevelUp();
					}
					check = true;
				}
			}

			if (check)
			{
				cost = new BigInteger(charValue.LevelUpBaseCost * (1 + charValue.LevelPerCostRate * charValue.Level.Value));
				enhanceText.text = $"강화\n {NumberTranslater.TranslateNumber(cost)}";
				
				DataManager.Instance.RefreshItemData(ItemType.Character,charValue);
				DataManager.Instance.SaveData();
			}
		});
		
		equipButton.onClick.AddListener(() =>
		{
			if (curCharacter.Value.IsLock.Value)
				return;

			Player.Instance.EquipCharacter(curCharacter.Value);
		});
	}
}