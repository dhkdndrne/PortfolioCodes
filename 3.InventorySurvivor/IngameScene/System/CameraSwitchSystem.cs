using System;
using UniRx;
using UnityEngine;

[Serializable]
public class CameraSwitchSystem
{
	[SerializeField] private Camera mainCam;
	[SerializeField] private Camera secondCam;
	[SerializeField] private GameObject mainCanvas;
	[SerializeField] private GameObject ArrangementCanvas;
	
	public void Init()
	{
		GameManager.Instance.Step.Subscribe(step =>
		{
			switch (step)
			{
				case GameStep.Playing:
					SwitchCamera(true);
					break;

				case GameStep.UnLockSlot:
					SwitchCamera(false);
					break;
			}
		}).AddTo(GameManager.Instance);
	}
	
	private void SwitchCamera(bool isMain)
	{
		mainCam.enabled = isMain;
		secondCam.enabled = !isMain;
		
		mainCanvas.SetActive(isMain);
		ArrangementCanvas.SetActive(!isMain);
	}
}