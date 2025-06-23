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
        if (!myAI.CheckTargetInAttackRange())
        {
            ChangeState<State_Move>();
            return;
        }
        
        //타겟 방향을 보고있지않으면 타겟을 바라보도록
        if (!IsLookTarget())
        {
            Vector3 myPos = transform.position;
            Vector3 targetPos = myAI.Target.transform.position;
            Vector3 dir = targetPos - myPos;
        
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(dir), Time.deltaTime * 5f);
        }
        // else if (!myAI.UnitBase.IsActivatingSkill && myAI.UnitBase.CanUseSkill())
        // {
        //     myAI.unitBase.OperateSkill();
        // }
        else if(myAI.UnitBase.CanAttack)
        {
           myAI.UnitBase.Attack();
        }
    }
    public override void EndState()
    {

    }

    private bool IsLookTarget()
    {
        // //타겟의 방향 
        // Vector3 targetDir = (myAI.Target.transform.position - transform.position).normalized;
        // float dot = Vector3.Dot(transform.forward, targetDir);
        //  
        // // 내적 값을 [-1, 1] 사이로 클램프
        // dot = Mathf.Clamp(dot, -1f, 1f);
        //
        // //내적을 이용한 각 계산하기
        // // thetha = cos^-1( a dot b / |a||b|)
        // float theta = Mathf.Acos(dot) * Mathf.Rad2Deg;
        //
        // //Debug.Log("타겟과 AI의 각도 : " + theta);
        // return theta <= 5f;

        return true;
    }
}
