using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/ Target_Block Data", fileName = "New Target_Block Data")]
public class Target_Block_Data : TargetData
{
	[SerializeField] private BlockData blockData;
	[SerializeField] private GameObject prefab;

	public ColorLayer ColorLayer => blockData.ColorLayer;
	public BlockData BlockData => blockData;
	public GameObject Prefab => prefab;
	public override Sprite GetSprite() => blockData.Sprite;

}