using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 유닛 배치 후 방향 설정하는 조이스틱의 움직임을 체크하고 방향을 전달하는 클래스
/// </summary>
public class DirectionJoystick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	private const float MAX_DIST = 218f;
	private const float MIN_DIST = -218f;
	private const float DRAG_DIST = 120f;

	private Canvas canvas;
	private Vector2 centerAnchoredPos;
	private RectTransform rt;

	private bool hasDirection;
	
	public event Action<OperatorDirection, bool> DRAG_ACTION;
	public event Action<bool> END_DRAG_ACTION;

	private void Awake()
	{
		canvas = transform.GetComponentInParent<Canvas>();
		rt = GetComponent<RectTransform>();
	}

	private void OnEnable()
	{
		rt.anchoredPosition = Vector2.zero;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			rt.parent as RectTransform, 
			eventData.position, 
			canvas.worldCamera, 
			out centerAnchoredPos
		);
		hasDirection = false;
	}

	public void OnDrag(PointerEventData eventData)
	{
		Vector2 localPoint;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			rt.parent as RectTransform, 
			eventData.position, 
			canvas.worldCamera, 
			out localPoint
		);

		Vector2 newPos = localPoint;

		rt.anchoredPosition = new Vector2(
			Mathf.Clamp(newPos.x, MIN_DIST, MAX_DIST),
			Mathf.Clamp(newPos.y, MIN_DIST, MAX_DIST)
		);

		// anchoredPosition으로 거리 계산
		if (Vector2.Distance(centerAnchoredPos, rt.anchoredPosition) >= DRAG_DIST)
		{
			Vector2 dir = (rt.anchoredPosition - centerAnchoredPos).normalized;
			OperatorDirection operatorDir = Mathf.Abs(dir.x) > Mathf.Abs(dir.y)
				? (dir.x > 0 ? OperatorDirection.Right : OperatorDirection.Left)
				: (dir.y > 0 ? OperatorDirection.Up : OperatorDirection.Down);

			hasDirection = true;
			DRAG_ACTION?.Invoke(operatorDir, true);
		}
		else
		{
			hasDirection = false;
			DRAG_ACTION?.Invoke(OperatorDirection.None, false);
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (!hasDirection) 
			rt.anchoredPosition = Vector2.zero;

		END_DRAG_ACTION?.Invoke(hasDirection);
	}

}