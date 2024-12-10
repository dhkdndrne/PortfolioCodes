using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Cell : GridObject
{
	public TextMeshPro tmp;
	[SerializeField] private GameObject shadow;
	public GameObject Shadow => shadow;
	
	//임시
	public bool isSpawnCell;
}
