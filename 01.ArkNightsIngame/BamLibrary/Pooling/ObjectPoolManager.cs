using System;
using System.Collections;
using System.Collections.Generic;
using Bam.Singleton;
using UnityEngine;

[Serializable]
public class PoolItem
{
	[SerializeField] private GameObject prefab;
	[SerializeField] private string key;
	[SerializeField] private int size;

	public GameObject Prefab => prefab;
	public int Size => size;
	public string Key => key;

	public PoolItem(GameObject prefab, int size = 10)
	{
		this.prefab = prefab;
		key = prefab.name;
		this.size = size;
	}
}

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
	[SerializeField] private List<PoolItem> initialPools; // 에디터에서 초기 풀 설정
	private Dictionary<string, IPool> poolDictionary = new Dictionary<string, IPool>();

	// 초기화
	protected override void Init()
	{
		foreach (var item in initialPools)
		{
			CreatePool(item.Key, new ObjectPool(item, transform));
		}
	}

	// 풀 생성
	private void CreatePool(string key, IPool pool)
	{
		if (!poolDictionary.ContainsKey(key))
		{
			poolDictionary[key] = pool;
		}
	}
	
	public GameObject Spawn(string key, Action<GameObject> onSpawn = null)
	{
		if (!poolDictionary.TryGetValue(key, out var pool))
		{
			Debug.Log($"{key} 키값이 없음");
			return null;
		}

		return pool.Spawn(onSpawn);
	}
	
	// 오브젝트 가져오기
	public GameObject Spawn(GameObject prefab,int size = 10, Action<GameObject> onSpawn = null)
	{
		string key = prefab.name;
		if (!poolDictionary.TryGetValue(key, out var pool))
		{
			//새로운 풀 생성
			AddNewPool(prefab,size);
			pool = poolDictionary[key];
		}

		return pool.Spawn(onSpawn);
	}
	
	// 오브젝트 반환
	public void DeSpawn(GameObject obj, Action<GameObject> onReturn = null)
	{
		if (!poolDictionary.TryGetValue(obj.name, out var pool))
		{
			Debug.LogWarning($"{obj.name}라는 키가 없어서 해당 오브젝트는 파괴됐습니다.");
			Destroy(obj);
			return;
		}

		onReturn?.Invoke(obj);
		pool.DeSpawn(obj);
	}

	private void AddNewPool(GameObject obj, int size)
	{
		string key = obj.name;

		if (poolDictionary.ContainsKey(key))
		{
			Debug.LogWarning($"{key} 풀은 이미 존재합니다.");
			return;
		}

		PoolItem newPoolItem = new PoolItem(obj, size);
		ObjectPool newObjectPool = new ObjectPool(newPoolItem, transform);
		poolDictionary.Add(key, newObjectPool);
	}
}