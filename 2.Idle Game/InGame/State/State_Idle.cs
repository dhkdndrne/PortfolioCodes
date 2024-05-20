using System.Collections;
using System.Collections.Generic;
using Bam.Extensions;
using UnityEngine;

public class State_Idle: State
{
	public override void BeginState()
	{

	}
	
	public override void UpdateState()
	{
		myAI.FindEnemy();
		
		if (myAI.Target == null) 
			return;

		if (myAI.CheckTargetInAttackRange())
		{
			ChangeState<State_Attack>();
		}
		else
		{
			ChangeState<State_Move>();
		}
	}
	
	public override void EndState()
	{

	}
}
