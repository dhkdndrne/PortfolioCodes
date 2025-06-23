using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Exusiai : Operator
{
	private int skillIndex;
	private Skill skill;
	
	public override void Init()
	{
		base.Init();
		#region 액티브 스킬

		skillIndex = 2;
		skill = skillIndex switch
		{
			0 => new Exusiai_Sk_1(),
			1 => new Exusiai_Sk_2(),
			2 => new Exusiai_Sk_3(),
			_ => null
		};

		skill?.Init(this,skillIndex);

		if (skill?.SpChargeType == SpChargeType.PerSecond)
			controller.OnSkillSpCharge += skill.ChargeSP;

		#endregion
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