using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Attack : State
{
    public override void BeginState()
    {
      
    }
    
    public override void UpdateState()
    {
        if(myAI.Target == null || myAI.Target.IsDead)
            myAI.FindEnemy();

        if (myAI.Target == null || !myAI.CheckTargetInAttackRange())
        {
            ChangeState<State_Move>();
            return;
        }

        if (myAI.unitBase.CanAttack())
        {
            myAI.unitBase.Attack();
        }
    }
    public override void EndState()
    {

    }
}
