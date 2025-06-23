using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DividePanelUI : MonoBehaviour
{
	[SerializeField] private TMP_InputField inputField;
	[SerializeField] private Button addButton;
	[SerializeField] private Button subtractButton;
	[SerializeField] private Button confirmButton;
	[SerializeField] private Button cancelButton;

	private int originIndex;
	private int targetIndex;
	private int maxAmount;
	private Action<int ,int ,int > divideAction;
	
	public void Init(Action<int,int,int> action)
	{
		divideAction = action;
		addButton.onClick.AddListener(() => ChangeAmount(1));
		subtractButton.onClick.AddListener(() => ChangeAmount(-1));

		confirmButton.onClick.AddListener(() =>
		{
			if (!int.TryParse(inputField.text, out int num))
				return;
            
			if (num <= 0)
				return;

			if (num > maxAmount)
				num = maxAmount;
			
			divideAction.Invoke(originIndex, targetIndex, num);
			gameObject.SetActive(false);
		});
		cancelButton.onClick.AddListener(() => gameObject.SetActive(false));
	}

	private void ChangeAmount(int value)
	{
		int amount = int.Parse(inputField.text);
		
		amount += value;

		if (amount > maxAmount)
			amount = 1;

		if (amount < 1)
			amount = maxAmount;

		inputField.text = amount.ToString();
	}

	public void OpenPanel(int originIndex,int targetIndex,int curAmount)
	{
		gameObject.SetActive(true);

		this.originIndex = originIndex;
		this.targetIndex = targetIndex;
        
		maxAmount = curAmount;
		inputField.text = "1";
	}
}