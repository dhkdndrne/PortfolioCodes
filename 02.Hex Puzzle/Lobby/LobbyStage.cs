using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;


public class LobbyStage : MonoBehaviour
{
	[SerializeField] private StageData stageData;
	[SerializeField] private TextMeshPro tmp;

	private void Start()
	{
		tmp.text = $"Stage {stageData.StageNum}";
	}
	
	private void OnMouseEnter()
	{
		transform.DOScale(1.1f,0.1f).SetEase(Ease.Linear);
	}

	private void OnMouseExit()
	{
		transform.DOScale(1.0f,0.1f).SetEase(Ease.Linear);
	}

	private void OnMouseUp()
	{
		StageManager.stageData = stageData;
		PopUpManager.Instance.OpenStageInfoPanel();
		//SceneManager.LoadScene("GameScene");
	}
}
