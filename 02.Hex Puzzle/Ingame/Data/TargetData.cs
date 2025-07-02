using System.Collections.Generic;
using UnityEngine;

public class TargetData : ScriptableObject
{
	[SerializeField] private TargetObjectType targetObjectType;
	public TargetObjectType TargetObjectType => targetObjectType;
	public virtual Sprite GetSprite()
	{
		return null;
	}
}

