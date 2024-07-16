using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUnEquipedItemHolder : MonoBehaviour
{
	[SerializeField] private Transform[] spawnPoints;

	public Transform GetSpawnPoint(int index) => spawnPoints[index];
}
