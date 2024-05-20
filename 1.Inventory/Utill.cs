using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Utill
{
	private static GraphicRaycaster gr;
	private static PointerEventData ped;
	private static List<RaycastResult> rrList = new List<RaycastResult>();
    
	static Utill()
	{
		gr = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
		ped = new PointerEventData(EventSystem.current);
	}

	public static T RaycastAndGetFirstComponent<T>() where T : Component
	{
		rrList.Clear();

		ped.position = Input.mousePosition;
		gr.Raycast(ped, rrList);

		if (rrList.Count == 0)
			return null;

		return rrList[0].gameObject.TryGetComponent(out T t) ? t : null;
	}
}
