using System;
using Bam.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OperatorInfoPanel : MonoBehaviour
{
	private enum FilterOption
	{
		Skill,
		CharTrait,
		Talent
	}

	[SerializeField] private GameObject panel;
	[SerializeField] private Image operatorImage;

	[Header("체력")]
	[SerializeField] private Image hpBar;
	[SerializeField] private RectTransform hpFollowUI;
	[SerializeField] private TextMeshProUGUI hpText;

	[Header("토글")]
	[SerializeField] private Toggle skill_Toggle;
	[SerializeField] private Toggle characterTrait_Toggle;
	[SerializeField] private Toggle talent_Toggle;

	[Header("정보창 오브젝트")]
	[SerializeField] private SkillInfoPanel skillInfoPanel;
	[SerializeField] private TraitInfoPanel charTraitInfoPanel;
	[SerializeField] private TalentInfoPanel talentInfoPanel;

	[Header("상단 오퍼레이터 정보")]
	[SerializeField] private Image classIcon;
	[SerializeField] private TextMeshProUGUI nameText;
	[SerializeField] private TextMeshProUGUI levelText;

	[Header("스텟 텍스트")]
	[SerializeField] private TextMeshProUGUI damageText;
	[SerializeField] private TextMeshProUGUI defenseText;
	[SerializeField] private TextMeshProUGUI magicResistanceText;
	[SerializeField] private TextMeshProUGUI blockCountText;

	[SerializeField] private AttackRangeUI attackRangeUI;
	
	private FilterOption curFilter = FilterOption.Skill;
	private Operator curOp;
	private const float HP_FOLLOWER_OFFSET = 8.34f;

	private void Start()
	{
		skill_Toggle.onValueChanged.AddListener(isOn => ChangeSlotFilter(isOn, FilterOption.Skill));
		characterTrait_Toggle.onValueChanged.AddListener(isOn => ChangeSlotFilter(isOn, FilterOption.CharTrait));
		talent_Toggle.onValueChanged.AddListener(isOn => ChangeSlotFilter(isOn, FilterOption.Talent));

		ClickChecker.Instance.onOperatorClicked += Show;

		foreach (var op in GameManager.Instance.OperatorManager.GetAllOperators())
		{
			op.OnDeath += () =>
			{
				if (op == curOp)
				{
					curOp = null;
					Close();
				}
			};
		}
	}
	private void Update()
	{
		if (curOp is null)
			return;

		hpBar.fillAmount = curOp.Attribute.HpRatio.Value;

		float fill = hpBar.fillAmount;
		float width = hpBar.rectTransform.rect.width;

		// 끝 지점 position 계산
		Vector2 newPos = new Vector2((fill * width) - HP_FOLLOWER_OFFSET, hpFollowUI.anchoredPosition.y);

		// follower UI 이동
		hpFollowUI.anchoredPosition = newPos;
		
		hpText.text = $"{curOp.Attribute.Hp.Value}/{curOp.Attribute.MaxHp.Value}";
	}
	private void ChangeSlotFilter(bool isOn, FilterOption option)
	{
		if (curFilter == option)
			return;

		if (isOn)
		{
			curFilter = option;
			ShowPanel(curFilter);
		}
	}

	private void ShowPanel(FilterOption filterOption)
	{
		skillInfoPanel.SetActive(filterOption == FilterOption.Skill);
		charTraitInfoPanel.SetActive(filterOption == FilterOption.CharTrait);
		talentInfoPanel.SetActive(filterOption == FilterOption.Talent);
	}

	public void Show(Operator op)
	{
		if (op is null)
		{
			Close();
			return;
		}

		panel.gameObject.SetActive(true);
		
		if (curOp == op)
			return;

		curOp = op;
		operatorImage.sprite = ImageManager.Instance.operatorImageDic[curOp.OperatorID].GetSprite(0);
		var skill = curOp.GetSkill();

		if (skill is null)
		{
			skill_Toggle.SetActive(false);
			characterTrait_Toggle.isOn = true;
		}
		else
		{
			skill_Toggle.SetActive(true);
			skill_Toggle.isOn = true;
			skillInfoPanel.UpdateUI(curOp);
		}

		//패널 업데이트
		charTraitInfoPanel.UpdateUI(curOp.SubProfessionData);
		talentInfoPanel.UpdateUI(curOp.Talents);
		
		if (curOp.OnTile == null) hpBar.fillAmount = 1;
		else hpBar.fillAmount = curOp.Attribute.HpRatio.Value;

		hpText.text = $"{curOp.Attribute.Hp.Value}/{curOp.Attribute.MaxHp.Value}";

		nameText.text = curOp.UnitName;
		levelText.text = "1"; //todo 추후 수정

		classIcon.sprite = ImageManager.Instance.battle_ClassSpriteDic[curOp.OperatorClass].classIcon;
		
		//스탯계산
		damageText.text = $"공격 {(int)curOp.GetFinalDamage()}";
		defenseText.text = $"방어 {(int)curOp.GetFinalMeleeDefense()}";
		magicResistanceText.text = $"마항 {(int)curOp.Attribute.MagicResistance}";
		blockCountText.text = $"저지 {curOp.MaxBlock}";
		
		//공격범위
		attackRangeUI.ShowAttackRange(curOp.GetOriginAttackRangeGrid()).Forget();
	}

	public void Close()
	{
		curOp = null;
		panel.gameObject.SetActive(false);
	}
}