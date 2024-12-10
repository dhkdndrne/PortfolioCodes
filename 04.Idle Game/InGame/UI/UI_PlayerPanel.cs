using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerPanel : MonoBehaviour, IPanel
{
	[SerializeField] private RectTransform panel;

	[SerializeField] private GameObject[] playerPanels;
	[SerializeField] private Button[] dockBtns;
	[SerializeField] private RectTransform selectedPanelHighLight;
    
	private readonly float[] HIGHLIGHT_POS_ARRAY = { -500f, -250f, 0, 250f, 500f };
	private readonly float TARGET_POSITION_Y = 180f;

	private float originPosY;
	private int curIndex = -1;
	
	private void Awake()
	{
		originPosY = panel.transform.position.y;
        
		for (int i = 0; i < dockBtns.Length; i++)
		{
			int index = i;
			dockBtns[index].onClick.AddListener(() =>
			{
				if (curIndex == index)
					return;
				
				if(curIndex != -1)
					playerPanels[curIndex].SetActive(false);

				curIndex = index;
				playerPanels[curIndex].SetActive(true);
				MoveHighlight(index).Forget();
			});
		}
		
	}

	private async UniTaskVoid MoveHighlight(int index)
	{
		await selectedPanelHighLight.DOAnchorPosX(HIGHLIGHT_POS_ARRAY[index],0.3f);
	}
	
	public void OpenPanel()
	{
		gameObject.SetActive(true);

		curIndex = 0;
		playerPanels[curIndex].SetActive(true);
		selectedPanelHighLight.anchoredPosition = new Vector2(HIGHLIGHT_POS_ARRAY[0], 5);
		
		UIManager.Instance.ClosePanel();
		UIManager.Instance.OpenPanel(this);
		panel.DOAnchorPosY(TARGET_POSITION_Y, 0.4f).SetEase(Ease.OutBack);
	}

	public void ClosePanel()
	{
		playerPanels[curIndex].SetActive(false);
		curIndex = -1;
		
		panel.DOAnchorPosY(originPosY, 0.4f).SetEase(Ease.OutBack);
		gameObject.SetActive(false);
	}

}