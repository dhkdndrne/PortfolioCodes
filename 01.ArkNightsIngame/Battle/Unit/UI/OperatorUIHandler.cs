using System;
using TMPro;
using UnityEngine;

/// <summary>
/// 오퍼레이터의 공격방향 화살표 스프라이트와
/// 체력바 및 스킬바 관리
/// </summary>
public class OperatorUIHandler : MonoBehaviour
{
	[SerializeField] private Transform directionArrow;
	
	[SerializeField] private HpSlider hpBar;
	[SerializeField] private SpSlider spBar;
	[SerializeField] private SliderHandler ShieldBar;
	
	[SerializeField] private GameObject skillReadyIcon;
	[SerializeField] private GameObject skillStackUI;
	[SerializeField] private TextMeshProUGUI skillStackText;
	
	public HpSlider HpBar => hpBar;
	public SpSlider SpBar => spBar;
	
	private IDisposable spRatioSubscription;
	private IDisposable skillStackSubscription;
	
	private void OnEnable()
	{
		hpBar.gameObject.SetActive(false);
		spBar.gameObject.SetActive(false);
		ShieldBar.gameObject.SetActive(false);
		skillReadyIcon.SetActive(false);
		skillStackUI.gameObject.SetActive(false);
	}

	public void Init(OperatorAttribute attribute,Skill skill = null)
	{
		hpBar.ChainEvent(attribute.HpRatio,1);
		hpBar.SetSliderActive(true);
		
		ShieldBar.ChainEvent(attribute.ShieldRatio,0);
		ShieldBar.SetSliderActive(true);
		
		if (skill != null)
		{
			spBar?.ChainEvent(skill.SpRatio,0);

			if (skill.ActiveType is SkillActiveType.Manual)
			{
				spRatioSubscription = skill.SpRatio.Subscribe(val =>
				{
					if (val >= 1f) skillReadyIcon?.SetActive(true);
					else skillReadyIcon?.SetActive(false);
				});
			}
			if (skill is StackSkill stackSkill)
			{
				skillStackSubscription = stackSkill.Stack.Subscribe(val =>
				{
					skillStackUI.SetActive(val > 0);
					skillStackText.text = val.ToString();
				});
			}
		}
		spBar?.SetSliderActive(skill != null);
	}

	public void UnChainEvent(OperatorAttribute attribute,Skill skill = null)
	{
		hpBar.UnChainEvent(attribute.HpRatio);
		ShieldBar.UnChainEvent(attribute.ShieldRatio);
		
		if (skill != null) 
			spBar?.UnChainEvent(skill.SpRatio);
		spRatioSubscription?.Dispose();
		skillStackSubscription?.Dispose();
	}
	
	public void SetDirectionArrowActivate(bool on)
	{
		directionArrow.gameObject.SetActive(on);
	}
	
}