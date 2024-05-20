using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
	[SerializeField] private Image slotBG;
	[SerializeField] private Image itemIcon;
	[SerializeField] private Image selectImage;
	[SerializeField] private Image lockImage;
	[SerializeField] private TextMeshProUGUI amountText;

	[SerializeField] private SlotRankImageData slotRankImageData;
	[Range(0, 1), SerializeField] private float targetAlphaValue; // 목표 알파값 0 ~ 100%
	[Range(0, 1), SerializeField] private float duration;         // 알파값 변화까지의 시간

	private Action<int> mouseEnterAction;
	private Action<int> mouseClickAction;

	public bool IsAppliedFilter { get; private set; }
	public int Index { get; private set; }

	public void Init(Action<int> enterAction, Action<int> clickAction, int index)
	{
		mouseEnterAction = enterAction;
		mouseClickAction = clickAction;
		Index = index;
		IsAppliedFilter = true;
	}

	public void UpdateSlotUI(Item item)
	{
		if (item == null || item.Data == null)
		{
			itemIcon.enabled = false;
			amountText.enabled = false;
			slotBG.sprite = slotRankImageData.GetBGSprite((int)ItemRank.Normal);
			return;
		}

		itemIcon.enabled = true;
		amountText.enabled = item.Data.MaxAmount != 1;
		
		slotBG.sprite = slotRankImageData.GetBGSprite((int)item.Data.Rank);

		itemIcon.sprite = item.Data.Sprite;
		amountText.text = item.Amount.ToString();
	}

	private IEnumerator CoFadeIn()
	{
		selectImage.gameObject.SetActive(true);
		Color tempColor = selectImage.color;

		float curAlpha = 0;
		float unit = targetAlphaValue / duration;

		while (curAlpha <= targetAlphaValue)
		{
			curAlpha += unit * Time.deltaTime;
			tempColor.a = curAlpha;
			selectImage.color = tempColor;

			yield return null;
		}
	}
	private IEnumerator CoFadeOut()
	{
		Color tempColor = selectImage.color;

		float curAlpha = targetAlphaValue;
		float unit = targetAlphaValue / duration;

		while (curAlpha > 0)
		{
			curAlpha -= unit * Time.deltaTime;
			tempColor.a = curAlpha;
			selectImage.color = tempColor;

			yield return null;
		}

		selectImage.gameObject.SetActive(false);
	}

	public void OnPointerEnter()
	{
		if (!IsAppliedFilter)
			return;
		
		StartCoroutine(nameof(CoFadeIn)); // FadeIn 코루틴을 실행시킨다.
		mouseEnterAction.Invoke(Index);   // 현재 슬롯 Index를 매개변수로 action을 Invoke 시킨다.
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		OnPointerEnter();
	}
	
	public void OnPointerExit(PointerEventData eventData)
	{
		if (!IsAppliedFilter)
			return;
		
		StopCoroutine(nameof(CoFadeIn));
		StartCoroutine(nameof(CoFadeOut));
	}
	
	public void OnPointerDown(PointerEventData eventData)
	{
		if (Input.GetMouseButtonDown(1) || lockImage.gameObject.activeSelf)
			return;
		
		mouseClickAction.Invoke(Index);
	}

	public void SetSlotLock(bool isLock)
	{
		lockImage.gameObject.SetActive(isLock);
	}
	
	/// <summary>
	/// 필터 적용에 따라 슬롯 색 변경
	/// </summary>
	/// <param name="isAppliedFilter"></param>
	public void SetSlotAccessState(bool isAppliedFilter)
	{
		if (IsAppliedFilter == isAppliedFilter)
			return;
		
		IsAppliedFilter = isAppliedFilter;
		
		slotBG.color = isAppliedFilter ? Color.white : Color.gray;
		itemIcon.color = isAppliedFilter ?  Color.white: Color.gray;
		amountText.color = isAppliedFilter ? Color.white: Color.gray;
	}
}