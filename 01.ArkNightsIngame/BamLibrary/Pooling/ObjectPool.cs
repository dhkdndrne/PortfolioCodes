using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : IPool
{
	private Queue<GameObject> pool = new Queue<GameObject>();
	private PoolItem poolItem;
	private Transform container;

	public ObjectPool(PoolItem item, Transform parent)
	{
		poolItem = item;
		GameObject obj = new GameObject(item.Prefab.name + "_POOL");

		container = obj.transform;
		container.SetParent(parent);

		for (int i = 0; i < poolItem.Size; i++)
		{
			pool.Enqueue(CreateNewObject());
		}
	}

	private GameObject CreateNewObject()
	{
		var obj = UnityEngine.Object.Instantiate(poolItem.Prefab, container, true);
		obj.name = poolItem.Key;
		obj.SetActive(false);
		return obj;
	}

	public GameObject Spawn(Action<GameObject> onSpawn = null)
	{
		if (pool.Count == 0)
		{
			pool.Enqueue(CreateNewObject());
		}

		var obj = pool.Dequeue();
		obj.SetActive(true);
		onSpawn?.Invoke(obj);
		
		return obj;
	}

	public void DeSpawn(GameObject obj,Action<GameObject> action = null)
	{
		pool.Enqueue(obj);
		obj.transform.SetParent(container);
		action?.Invoke(obj);
		
		obj.SetActive(false);
	}
}