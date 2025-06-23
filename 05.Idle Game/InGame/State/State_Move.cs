using System.Collections;
using System.Collections.Generic;
using Bam.Extensions;
using UnityEngine;

public class State_Move : State
{
	public override void BeginState()
	{
		animator.SetBool(AnimationHash.MOVE_ANIM_HASH, true);
	}
	public override void UpdateState()
	{
		myAI.FindEnemy();

		if (myAI.CheckTargetInAttackRange())
		{
			ChangeState<State_Attack>();
		}
		else
		{
			var dir = myAI.UnitAffiliation is UnitAffiliation.Player ? Vector3.right : Vector3.left;
			transform.position += dir * (myAI.unitBase.Stat.MoveSpeed * Time.deltaTime);
		}
	}

	public override void EndState()
	{
		animator.SetBool(AnimationHash.MOVE_ANIM_HASH, false);
	}
}