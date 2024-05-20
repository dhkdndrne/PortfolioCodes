using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GhostItemUI : MonoBehaviour
{
	[SerializeField] private Image icon;
	[SerializeField] private TextMeshProUGUI text;
	[SerializeField] private RectTransform parentRT;
	
	private RectTransform rt;
	private Action<int> dragEndAction;

	public void Init(Action<int> action)
	{
		dragEndAction = action;
	}

	private void Awake()
	{
		rt = GetComponent<RectTransform>();
	}

	/// <summary>
	/// 더미 이미지 수량 및 아이콘 변경 함수
	/// </summary>
	/// <param name="item"></param>
	public void ChangeUI(Item item)
	{
		if (item == null)
		{
			gameObject.SetActive(false);
			return;
		}

		gameObject.SetActive(true);

		icon.sprite = item.Data.Sprite;
        
		text.enabled = item.Data.MaxAmount != 1; // 최대 수량이 1인경우 텍스트를 비활성화 시켜준다.
		text.text = item.Amount.ToString();

		StartCoroutine(nameof(CoDrag));
	}

	private IEnumerator CoDrag()
	{
		//마우스 좌클릭 하고있는 동안 실행
		while (Input.GetMouseButton(0))
		{
			//캔버스에서 RectTransform이 Screen Space-Overlay 모드로 설정된 경우 cam 매개 변수는 null 이어야 한다.
			Vector3 mousePos = Input.mousePosition;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				parentRT, mousePos, null, out Vector2 localPoint);

			rt.anchoredPosition = localPoint;
			yield return null;
		}

		var slot = Utill.RaycastAndGetFirstComponent<SlotUI>();
		int index = slot == null ? -1 : slot.Index;
		dragEndAction.Invoke(index);
	}

}