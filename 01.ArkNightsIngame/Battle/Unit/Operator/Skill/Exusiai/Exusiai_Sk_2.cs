using System.Collections.Generic;
using UnityEngine;

public class Exusiai_Sk_2 : Skill
{
	private float interval;
	private bool isAttacking;

	public override void SetSkill()
	{
		isActivating = true;
		isAttacking = false;
		isEnd = false;
		
		elapsedTime = duration;
		base.SetSkill();
	}
	public override void Use(Operator caster, HashSet<Unit> targetList)
	{
		foreach (var target in targetList)
		{
			for (int i = 0; i < 4; i++)
			{
				target.Hit(caster);
			}
			break;
		}
	}
	public override bool IsSkillEnd()
	{
		if (isEnd)
		{
			isActivating = false;
			isAttacking = false;
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

		if (interval <= 0 && !isAttacking && targetList.Count > 0)
		{
			isAttacking = true;
			return true;
		}

		if (interval > 0)
			interval -= Time.deltaTime;

		elapsedTime -= Time.deltaTime;
		DurationRatio.Value = elapsedTime / duration;

		return false;
	}
}
