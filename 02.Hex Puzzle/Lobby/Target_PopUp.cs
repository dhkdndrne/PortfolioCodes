using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Target_PopUp : PopUP
{
	[SerializeField] private TextMeshProUGUI stageText;
	[SerializeField] private GameObject[] targetObjects;
	[SerializeField] private Button startButton;

	private TargetUIToken[] targetTokens;
	
	private void Awake()
	{
		targetTokens = new TargetUIToken[targetObjects.Length];
		for (int i = 0; i < targetObjects.Length; i++)
		{
			targetTokens[i] = new TargetUIToken()
			{
				image = targetObjects[i].GetComponentInChildren<Image>(),
				text = targetObjects[i].GetComponentInChildren<TextMeshProUGUI>()
			};
		}
		
		startButton.onClick.AddListener(() => SceneManager.LoadScene("GameScene"));
	}

	private void SetTargetUI()
	{
		var stageData = StageManager.stageData;
		stageText.text = $"Stage {stageData.StageNum}";

		var targets = stageData.GetTargetList();

		for (int i = 0; i < targets.Count; i++)
		{
			targetObjects[i].SetActive(true);
			
			targetTokens[i].image.sprite = targets[i].targetData.Sprites[0];

			if (targets[i].targetData.TargetObjectType is TargetObjectType.Block)
			{
				var targetBlockData = (Target_Block_Data)targets[i].targetData;
				targetTokens[i].image.color = ColorManager.GetColor(targetBlockData.ColorLayer);
			}
			
			targetTokens[i].text.text = targets[i].count.ToString();
		}
	}
	public override void Open()
	{
		gameObject.SetActive(true);

		ResetTargetUI();
		SetTargetUI();

		transform.localScale = Vector3.zero;
		transform.DOScale(Vector3.one, 0.7f).SetEase(Ease.OutBounce);
	}

	public override void Close()
	{
		gameObject.SetActive(false);
		PopUpManager.Instance.OnPopUpClosed.Invoke();
	}

	private class TargetUIToken
	{
		public Image image;
		public TextMeshProUGUI text;
	}

	private void ResetTargetUI()
	{
		for (int i = 0; i < targetObjects.Length; i++)
		{
			targetObjects[i].SetActive(false);
			targetTokens[i].image.color = Color.white;
		}
	}
}