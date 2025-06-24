using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedOperatorUI : MonoBehaviour
{
	[Header("GameObjects")]
	[SerializeField] private GameObject panelObject;
	[SerializeField] private Transform movePanel;
	[SerializeField] private GameObject fullCostIcon;
	[SerializeField] private GameObject autoSkillIcon;

	[Header("UI")]
	[SerializeField] private TextMeshProUGUI skillCostText;
	[SerializeField] private Image skillCostFill;
	[SerializeField] private Image skillDurationFill;
	[SerializeField] private Button skillUseButton;
	[SerializeField] private Button retreatButton;

	[SerializeField] private GameObject stackBG;
	[SerializeField] private TextMeshProUGUI stackText;

	private Operator currentOperator;

	private void Awake()
	{
		retreatButton.onClick.AddListener(() =>
		{
			if (AutoBattleManager.Instance.IsReplayMode.Value)
				return;

			if (currentOperator != null)
			{
				AutoBattleManager.Instance.RecordEvent(new TimelineEvent(currentOperator.OperatorID, TimeManager.Instance.Frame, ReplayEventType.Retreat));
				currentOperator.Controller.Retreat();
			}
			AttackRangeIndicator.Instance.DisableAttackRange();
			TimeManager.Instance.StopSlowMotion();
			Close();
		});

		var operators = GameManager.Instance.OperatorManager.GetAllOperators();
		foreach (var op in operators)
		{
			op.OnDeath += () =>
			{
				if (currentOperator != null && op.OperatorID == currentOperator.OperatorID)
				{
					OnOperatorDeath();
					Close();
				}
			};
		}

		ClickChecker.Instance.onOperatorClicked += ShowUI;
	}

	public void ShowUI(Operator op)
	{
		if (op == null)
		{
			TimeManager.Instance.StopSlowMotion();
			Close();
			return;
		}
		
		TimeManager.Instance.StartSlowMotion();
		//선택한 오퍼레이터와 이전에 클릭한 오퍼레이터가 다를때
		if (currentOperator != op)
		{
			UnsubscribeCurrentOperator();

			autoSkillIcon.SetActive(false);
			fullCostIcon.SetActive(false);
			skillUseButton.onClick.RemoveAllListeners();

			currentOperator = op;
			var skill = currentOperator.GetSkill();
			if (skill != null)
			{
				SubscribeSkillEvents(skill);

				// 스킬버튼 이미지 변경
				skillUseButton.image.sprite = ImageManager.Instance.operatorImageDic[op.OperatorID].GetSkillIcon(skill.Index);

				// 초기 UI 상태 설정
				skillCostFill.gameObject.SetActive(!skill.IsActivating && !skill.CanUse());
				skillDurationFill.gameObject.SetActive(skill.IsActivating);

				if (skill.ActiveType == SkillActiveType.Auto)
					autoSkillIcon.SetActive(true);

				skillUseButton.onClick.AddListener(() =>
				{
					if (AutoBattleManager.Instance.IsReplayMode.Value)
						return;

					if (skill.CanUse())
					{
						AutoBattleManager.Instance.RecordEvent(new TimelineEvent(currentOperator.OperatorID, TimeManager.Instance.Frame, ReplayEventType.UseSkill));

						currentOperator.Controller.UseSkillManually();
						fullCostIcon.SetActive(false);
						skillDurationFill.gameObject.SetActive(true);
						
						TimeManager.Instance.StopSlowMotion();
						AttackRangeIndicator.Instance.DisableAttackRange();
						Close();
					}
				});

				if (skill is StackSkill stackSkill)
				{
					stackBG.SetActive(true);
					stackText.text = stackSkill.Stack.Value.ToString();
				}
				else stackBG.SetActive(false);
			}
		}

		movePanel.position = Camera.main.WorldToScreenPoint(op.transform.position);
		panelObject.SetActive(true);
	}

	public void Close() => panelObject.SetActive(false);

	private void OnOperatorDeath()
	{
		if (currentOperator != null)
		{
			var skill = currentOperator.GetSkill();
			if (skill != null)
				UnsubscribeSkillEvents(skill);
		}

		currentOperator = null;
	}

	private void UnsubscribeCurrentOperator()
	{
		if (currentOperator != null)
		{
			var skill = currentOperator.GetSkill();
			if (skill != null)
				UnsubscribeSkillEvents(skill);

			currentOperator = null;
		}
	}

	private void SubscribeSkillEvents(Skill skill)
	{
		skill.Sp.Subscribe(UpdateText);
		skill.SpRatio.Subscribe(UpdateFillAmount);
		skill.DurationRatio.Subscribe(UpdateDurationFill);

		if (skill is StackSkill stackSkill)
		{
			stackSkill.Stack.Subscribe(val =>
			{
				UpdateStackText(val);
			});
			stackBG.SetActive(true);
		}
	}

	private void UnsubscribeSkillEvents(Skill skill)
	{
		skill.Sp.Unsubscribe(UpdateText);
		skill.SpRatio.Unsubscribe(UpdateFillAmount);
		skill.DurationRatio.Unsubscribe(UpdateDurationFill);

		if (skill is StackSkill stackSkill)
		{
			stackBG.SetActive(false);
			stackSkill.Stack.Unsubscribe(UpdateStackText);
		}
	}
	private void UpdateStackText(int val)
	{
		stackText.text = val.ToString();
	}

	private void UpdateText(float val)
	{
		int requiredSp = currentOperator?.GetSkill()?.RequiredSp ?? 0;
		skillCostText.text = $"{(int)val}/{requiredSp}";
	}

	private void UpdateFillAmount(float val)
	{
		skillCostFill.fillAmount = val;

		if (val >= 1f)
		{
			var skill = currentOperator.GetSkill();
			skillCostFill.gameObject.SetActive(false);

			if (skill != null && skill.ActiveType == SkillActiveType.Auto)
			{
				autoSkillIcon.SetActive(true);
			}
			else
			{
				skillUseButton.image.color = Color.white;
				fullCostIcon.SetActive(true);
			}
		}
		else
		{
			skillUseButton.image.color = new Color(135 / 255f, 135 / 255f, 135 / 255f, 1f);
		}
	}

	private void UpdateDurationFill(float val)
	{
		skillDurationFill.fillAmount = val;

		if (val <= 0)
		{
			skillCostFill.gameObject.SetActive(true);
			skillDurationFill.gameObject.SetActive(false);
		}
	}
}