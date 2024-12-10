using System.Collections;
using System.Collections.Generic;
using Bam.Extensions;
using UnityEngine;

public class State_Move : State
{
	public override void BeginState()
	{
		myAI.Animator.SetBool(Define.MOVE_ANIM_HASH, true);
	}
	public override void UpdateState()
	{
		if (myAI.CheckTargetInAttackRange())
		{
			ChangeState<State_Attack>();
		}
		else
		{
			Vector3 myPos = transform.position;
			Vector3 targetPos = myAI.Target.transform.position;
			
			Vector3 dir = targetPos - myPos;
		
			transform.SetPositionAndRotation(Vector3.MoveTowards(myPos, targetPos, myAI.UnitBase.MoveSpeed * Time.deltaTime),
				Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f));
		}

	}

	public override void EndState()
	{
		myAI.Animator.SetBool(Define.MOVE_ANIM_HASH, false);
	}
}