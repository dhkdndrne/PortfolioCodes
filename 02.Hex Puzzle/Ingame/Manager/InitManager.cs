using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitManager : MonoBehaviour
{
	[SerializeField] private List<ManagerToken> list;

	private void Start()
	{
		list.Sort((x, y) => x.order.CompareTo(y.order));

		foreach (var m in list)
		{
			m.obj.GetComponent<IManger>().InitManager();
		}
		
	}
}
[Serializable]
public struct ManagerToken
{
	public int order;
	public GameObject obj;
}