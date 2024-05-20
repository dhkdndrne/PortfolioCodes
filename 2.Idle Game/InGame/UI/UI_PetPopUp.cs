using System.Text;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_PetPopUp : MonoBehaviour
{
	[SerializeField] private Image rankBG, itemIcon;
	[SerializeField] private Slider pieceSlider;
	[SerializeField] private TextMeshProUGUI itemNameTxt, rankTxt, levelTxt, pieceTxt, equippedEffectTxt, ownEffectTxt;
	[SerializeField] private Button equipBtn, enhanceBtn;

	[Header("장착버튼 텍스트"), SerializeField] 
	private TextMeshProUGUI equipBtnText;
	
	private StringBuilder sb;
	private ReactiveProperty<Item_Pet> curPet = new();

	private void Awake()
	{
		sb = new StringBuilder();

		curPet.Subscribe(item =>
		{
			item?.Level.Subscribe(val =>
			{
				levelTxt.text = $"LV {val}";
			}).AddTo(this);

			item?.piece.Subscribe(val =>
			{
				pieceTxt.text = $"{val} / {item.MaxPiece}";
				pieceSlider.value = (float)val / item.MaxPiece;

				enhanceBtn.interactable = item.IsLock.Value || item.piece.Value < item.MaxPiece ? false : true;
				enhanceBtn.image.material = item.piece.Value < item.MaxPiece ? ImageManager.Instance.GrayScaleMaterial : null;
			}).AddTo(this);
            
		}).AddTo(this);

		equipBtn.onClick.AddListener(EquipPet);
	}

	public void OpenPopUp(ItemBase item)
	{
		gameObject.SetActive(true);

		curPet.Value = item as Item_Pet;

		itemNameTxt.text = item.ItemName;
		rankTxt.text = item.Rank.GetRankTypeToString();

		sb.Clear();
		sb.Append(item.OwnEffect.EffectType.GetOwnEffectTypeToString());
		sb.Append($" <color=#30BF3C>+{NumberTranslater.TranslateNumber(item.OwnEffect.Effect)}%</color>");
		ownEffectTxt.text = sb.ToString();

		rankBG.sprite = ImageManager.Instance.GetItemRankBg(item.Rank);
		itemIcon.sprite = ImageManager.Instance.GetItemIcon(ItemType.Pet, item.ID);
		equipBtnText.text = curPet.Value.IsEquipped ? "해제" : "장착";
		equippedEffectTxt.text = curPet.Value.GetDescription();
	}

	private void EquipPet()
	{
		if(curPet.Value.IsEquipped)
			Player.Instance.UnEquipPet(curPet.Value.ID);
		else
			Player.Instance.EquipPet(curPet.Value, -1);
		
		gameObject.SetActive(false);
	}
}