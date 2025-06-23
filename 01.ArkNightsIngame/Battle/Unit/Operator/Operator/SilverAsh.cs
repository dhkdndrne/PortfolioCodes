using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SilverAsh : Operator
{
	private int skillIndex;
	private Skill skill; 
	
	public override void Init()
	{
		base.Init();
		skillIndex = 2;
		skill = skillIndex switch
		{
			0 => new SilverAsh_Sk_1(),
			1 => new SilverAsh_Sk_1(),
			2 => new SilverAsh_Sk_3(),
			_ => null
		};
		
		Debug.Assert(skill == null, "스킬 인덱스오류 : Skill is null");
		skill?.Init(this,skillIndex);
		
		if (skill?.SpChargeType == SpChargeType.PerSecond)
			controller.OnSkillSpCharge += skill.ChargeSP;
	}
	public override void Attack(HashSet<Unit> targetList)
	{
		Unit target = targetList.FirstOrDefault();

		if (target != null)
		{
			target.Hit(this);
		
			if (skill?.SpChargeType == SpChargeType.Attacking)
				skill.ChargeSP(1);
		}
	}
	
	public override Skill GetSkill()
	{
		return skill;
	}
}