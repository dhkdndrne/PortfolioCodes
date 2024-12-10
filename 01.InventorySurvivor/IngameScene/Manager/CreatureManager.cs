using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureManager : Manager<CreatureManager>
{
	[field: SerializeField] public Player Player { get; private set; }

	public List<Enemy> EnemyList { get; private set; }
	
	public override void OnStartManager()
	{
		EnemyList = new List<Enemy>();
	}
}