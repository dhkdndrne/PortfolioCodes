using System.Text;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterPanel : MonoBehaviour
{
	[Header("보유효과 버튼")]
	[SerializeField] private Button[] ownEffectButtons;

	[Header("보유 효과 클릭 팝업")]
	[SerializeField] private RectTransform ownInfoPopup;
	[SerializeField] private TextMeshProUGUI ownInfoText;
	[SerializeField] private Vector2[] popUpPos;

	[Header("고유스킬")]
	[SerializeField] private Image skillIcon;
	[SerializeField] private TextMeshProUGUI skillDescriptionText;

	[Header("아이템")]
	[SerializeField] private Transform[] equippedEquipmentItems;


	private OwnEffect[] ownEffects;
	private StringBuilder sb;
	private int curIndex;

	private void Awake()
	{
		ChangeInfo();
		Init();
	}

	private void Init()
	{
		sb = new StringBuilder();

		#region 보유효과 버튼들 초기화

		for (int i = 0; i < ownEffectButtons.Length; i++)
		{
			int index = i;
			ownEffectButtons[index].OnClickAsObservable().Subscribe(_ =>
			{
				if (index == curIndex && ownInfoPopup.gameObject.activeSelf)
				{
					ownInfoPopup.gameObject.SetActive(false);
					return;
				}

				curIndex = index;
				sb.Clear();
				ownInfoPopup.gameObject.SetActive(true);
				ownInfoPopup.anchoredPosition = popUpPos[index];

				string effectType = ownEffects[index].EffectType.GetOwnEffectTypeToString();

				sb.Append(effectType).Append(" ");
				sb.Append($"{NumberTranslater.TranslateNumber(ownEffects[index].Effect)}% 증가");

				ownInfoText.text = sb.ToString();
			});
		}


		#endregion

		sb.Clear();

		#region 고유스킬 UI 초기화

		var originalSkill = Player.Instance.EquippedCharacter.OriginalSkill;
		skillIcon.sprite = ImageManager.Instance.GetItemIcon(ItemType.Skill,originalSkill.ID);

		sb.Append("<SIZE=50><color=#AB7D28>").Append(originalSkill.ItemName).Append("</color></SIZE> \n \n");
		sb.Append(originalSkill.GetDescription());
		skillDescriptionText.text = sb.ToString();

		#endregion

		#region 플레이어장비 UI 초기화

		//플레이어 아이템 바뀌면 바뀌도록 설정
		Player.Instance.EquipmentChanged.Subscribe(value =>
		{
			var equippedItem = Player.Instance.GetEquipment(value);

			var level = equippedItem.Level;
			var rank = equippedItem.Rank;

			int index = value == EquipmentType.Weapon? 0 : 1;

			equippedEquipmentItems[index].GetComponent<Image>().sprite = ImageManager.Instance.GetItemRankBg(rank);                                        // 배경
			equippedEquipmentItems[index].GetChild(0).GetComponent<Image>().sprite = ImageManager.Instance.GetItemIcon(ItemType.Equipment,equippedItem.ID); //아이템 아이콘
			equippedEquipmentItems[index].GetChild(1).GetComponent<TextMeshProUGUI>().text = $"LV {level}";                                                //레벨 텍스트
			
			DataManager.Instance.SaveData();
		}).AddTo(this);
        
		//처음 한번만 직접 초기화
		int tempIndex = 0;
		for (int i = 0; i < equippedEquipmentItems.Length; i++)
		{
			var equippedItem = Player.Instance.GetEquipment((EquipmentType)tempIndex);

			var level = equippedItem.Level;
			var rank = equippedItem.Rank;

			equippedEquipmentItems[tempIndex].GetComponent<Image>().sprite = ImageManager.Instance.GetItemRankBg(rank);                                        // 배경
			equippedEquipmentItems[tempIndex].GetChild(0).GetComponent<Image>().sprite = ImageManager.Instance.GetItemIcon(ItemType.Equipment,equippedItem.ID); //아이템 아이콘
			equippedEquipmentItems[tempIndex].GetChild(1).GetComponent<TextMeshProUGUI>().text = $"LV{level}";                                                 //레벨 텍스트

			tempIndex++;
		}
        
		#endregion

	}
    
	private void ChangeInfo()
	{
		ImageManager imageManager = ImageManager.Instance;
		ownEffects = Player.Instance.EquippedCharacter.OwnEffects;

		for (int i = 0; i < ownEffectButtons.Length; i++)
		{
			ownEffectButtons[i].image.sprite = imageManager.GetOwnEffectTypeIcon(ownEffects[i].EffectType);
		}
	}
}