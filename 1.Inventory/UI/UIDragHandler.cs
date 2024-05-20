using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragHandler : MonoBehaviour, IDragHandler
{
	[SerializeField] private RectTransform rt;
	private Canvas canvas;
	
	private void Awake()
	{
		if (rt == null)
			rt = transform.parent.GetComponent<RectTransform>();

		canvas = GetComponentInParent<Canvas>();
	}

	public void OnDrag(PointerEventData eventData)
	{
		rt.anchoredPosition += eventData.delta / canvas.scaleFactor;
	}
}