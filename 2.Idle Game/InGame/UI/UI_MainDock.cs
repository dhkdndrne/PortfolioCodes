using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainDock : MonoBehaviour
{
	[SerializeField] private RectTransform[] dockRectTrs; //하단 버튼들 RectTransform

	private Button[] originButtons;
	private Button[] closeButtons;

	private int curIndex;

	[SerializeField] private float originDockSize;
	[SerializeField] private float changeDockSizeRate;
	private void Start()
	{
		int length = dockRectTrs.Length;
		originButtons = new Button[length];
		closeButtons = new Button[length];

		for (int i = 0; i < length; i++)
		{
			int index = i;

			originButtons[i] = dockRectTrs[i].GetChild(0).GetComponent<Button>();
			closeButtons[i] = dockRectTrs[i].GetChild(1).GetComponent<Button>();

			originButtons[i].OnClickAsObservable().Subscribe(_ =>
			{
				if (curIndex != index)
				{
					originButtons[curIndex].gameObject.SetActive(true);
					closeButtons[curIndex].gameObject.SetActive(false);
				}

				curIndex = index;

				originButtons[index].gameObject.SetActive(false);
				closeButtons[index].gameObject.SetActive(true);

				ChangeDockSize(index);
			}).AddTo(this);

			closeButtons[i].OnClickAsObservable().Subscribe(_ =>
			{
				originButtons[index].gameObject.SetActive(true);
				closeButtons[index].gameObject.SetActive(false);

				ResetDockSize();

				UIManager.Instance.ClosePanel();
			}).AddTo(this);
		}

		originDockSize = dockRectTrs[0].sizeDelta.x;
		changeDockSizeRate = originDockSize - 20f;
	}
	private void ChangeDockSize(int targetIndex)
	{
		for (int i = 0; i < dockRectTrs.Length; i++)
		{
			dockRectTrs[i].DOSizeDelta(new Vector2(i == targetIndex ? originDockSize + (20 * dockRectTrs.Length - 1): changeDockSizeRate, dockRectTrs[i].sizeDelta.y), 0.1f);
		}
	}

	private void ResetDockSize()
	{
		for (int i = 0; i < dockRectTrs.Length; i++)
		{
			dockRectTrs[i].DOSizeDelta(new Vector2(originDockSize, dockRectTrs[i].sizeDelta.y), 0.1f);
		}
	}
}