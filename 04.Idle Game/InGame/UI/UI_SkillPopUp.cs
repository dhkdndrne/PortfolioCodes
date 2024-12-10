using System.Text;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillPopUp : MonoBehaviour
{
	[SerializeField] private Image rankBG, skillIcon;
	[SerializeField] private Slider pieceSlider;
	[SerializeField] private TextMeshProUGUI skillNameTxt, rankTxt, levelTxt, pieceTxt, coolTimeTxt, descriptionTxt, ownEffectTxt;
	[SerializeField] private Button equipBtn, enhanceBtn;
	
	[Header("장착버튼 텍스트"), SerializeField] 
	private TextMeshProUGUI equipBtnText;
    
	private StringBuilder sb;
	private ReactiveProperty<Item_Skill> curSkill = new();

	private void Awake()
	{
		sb = new StringBuilder();
		equipBtn.onClick.AddListener(EquipSkill);

		enhanceBtn.OnClickAsObservable().Where(_ => curSkill != null).Subscribe(_ =>
		{
			curSkill.Value.LevelUp();
			DataManager.Instance.RefreshItemData(ItemType.Skill,curSkill.Value);
			DataManager.Instance.SaveData();
		}).AddTo(this);
		
		curSkill.Subscribe(skill =>
		{
			skill?.Level.Subscribe(val =>
			{
				levelTxt.text = $"LV {val}";
				coolTimeTxt.text = $"{skill.Skill.CoolTime}초";
				descriptionTxt.text = skill.GetDescription();
			}).AddTo(this);
            
			skill?.piece.Subscribe(val =>
			{
				pieceTxt.text = $"{val} / {skill.MaxPiece}";
				pieceSlider.value = (float)val / skill.MaxPiece;
                
				enhanceBtn.interactable = skill.IsLock.Value || skill.piece.Value < skill.MaxPiece ? false : true;
				enhanceBtn.image.material = skill.piece.Value < skill.MaxPiece ? ImageManager.Instance.GrayScaleMaterial : null;
			}).AddTo(this);
            
		}).AddTo(this);
	}

	public void OpenPopUp(ItemBase skill)
	{
		gameObject.SetActive(true);

		curSkill.Value = skill as Item_Skill;

		skillNameTxt.text = skill.ItemName;
		rankTxt.text = skill.Rank.GetRankTypeToString();

		sb.Clear();
		sb.Append(skill.OwnEffect.EffectType.GetOwnEffectTypeToString());
		sb.Append($" <color=#30BF3C>+{NumberTranslater.TranslateNumber(skill.OwnEffect.Effect)}%</color>");
		ownEffectTxt.text = sb.ToString();

		rankBG.sprite = ImageManager.Instance.GetItemRankBg(skill.Rank);
		skillIcon.sprite = ImageManager.Instance.GetItemIcon(ItemType.Skill,skill.ID);

		equipBtnText.text = curSkill.Value.IsEquipped ? "해제" : "장착";
	}

	private void EquipSkill()
	{
		if(curSkill.Value.IsEquipped)
			Player.Instance.SkillSystem.UnEquipSkill(curSkill.Value.ID);
		else
			Player.Instance.SkillSystem.EquipSkill(curSkill.Value, -1);
		
		gameObject.SetActive(false);
	}
}