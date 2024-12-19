using System.Collections.Generic;
using UnityEngine;

public class TargetData : ScriptableObject
{
	[SerializeField] private TargetObjectType targetObjectType;
	[SerializeField] private List<Sprite> sprites;
	
	public List<Sprite> Sprites => sprites;
	public TargetObjectType TargetObjectType => targetObjectType;
}

