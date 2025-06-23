using System;
using UnityEngine;

public interface IPool
{
	public void DeSpawn(GameObject obj,Action<GameObject> action = null);
	public GameObject Spawn(Action<GameObject> onSpawn = null);
}
