using System.Collections.Generic;
using UnityEngine;

public class Myrtle_Sk_1 : Skill
{
    private float interval;
    private float tick;
    private int cost;
    
    public override void SetSkill()
    {
        isActivating = true;
        isEnd = false;
		
        elapsedTime = duration;
      
        var data = DataManager.Instance.OperatorData.GetOperatorSkillData(id);
        cost = (int)data.GetSkillLevelData(10).Attributes[0].Value;
        
        interval = duration / cost;
        tick = interval;
        op.Controller.SetAnimationLoop(index,true);
        op.SubProfessionData.ApplyTrait(op.Controller);
        base.SetSkill();
    }
    
    public override void Use(Operator caster, HashSet<Unit> targetList)
    {
        GameManager.Instance.Stage.ChangeCostValue(1);
    }
    public override bool IsSkillEnd()
    {
	    if (isEnd)
	    {
		    isActivating = false;
		    op.Controller.SetAnimationLoop(index,false);
	    }
	    return isEnd;
    }
    public override bool UpdateSkill(HashSet<Unit> targetList)
    {
	    if (elapsedTime <= 0)
	    {
		    isEnd = true;
		    return false;
	    }

	    switch (tick)
	    {
		    case <= 0:
			    tick = interval;
			    return true;
		    case > 0:
			    tick -= Time.deltaTime;
			    break;
	    }

	    elapsedTime -= Time.deltaTime;
	    DurationRatio.Value = elapsedTime / duration;

	    return false;
    }
    public override void EndSkillAnim()
    {
	    interval = duration / cost;
    }
}
