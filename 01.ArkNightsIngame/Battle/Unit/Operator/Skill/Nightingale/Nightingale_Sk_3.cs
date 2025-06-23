using System.Collections.Generic;
using UnityEngine;

public class Nightingale_Sk_3 : Skill
{
    public override void Use(Operator caster, HashSet<Unit> targetList)
    {
       
    }
    public override bool IsSkillEnd()
    {
	    return true;
    }
    public override bool UpdateSkill(HashSet<Unit> targetList)
    {
	    return true;
    }
}
