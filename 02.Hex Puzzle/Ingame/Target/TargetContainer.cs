using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bam.Puzzle.Target
{
	[Serializable]
	public class TargetContainer
	{ 
		public string name = "";
		public CollectingTypes collectType;
		public TargetCountType countType;
	
		public TargetToken targetToken = new TargetToken();
		public GameObject prefab;
	}
	[Serializable]
	public class TargetToken
	{
		public List<Sprite> sprites;
		public ColorLayer colorLayer;
	}

	public enum TargetCountType
	{
		Manually,
		FromLevel
	}    
	public enum CollectingTypes
	{
		Destroy,
		ReachBottom,
		Spread,
		Clear
	}

}

