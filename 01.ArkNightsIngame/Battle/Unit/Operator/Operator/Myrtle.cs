using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Myrtle : Operator
{
    private int skillIndex;
    private Skill skill;

    public override void Init()
    {
        base.Init();
        skillIndex = 0;
        skill = skillIndex switch
        {
            0 => new Myrtle_Sk_1(),
            //1 => new Exusiai_Sk_2(),
            _ => null
        };
        
        skill?.Init(this, skillIndex);
        
        if (skill?.SpChargeType == SpChargeType.PerSecond)
            controller.OnSkillSpCharge += skill.ChargeSP;
    }
    public override void Attack(HashSet<Unit> targetList)
    {
        Unit target = targetList.FirstOrDefault();

        if (target != null)
        {
            target.Hit(this);
        }
    }

    public override Skill GetSkill()
    {
        return skill;
    }
}
