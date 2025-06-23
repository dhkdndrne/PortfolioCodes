using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Nightingale : Operator
{
	private int skillIndex;
	private Skill skill;
	[SerializeField] private GameObject attackFX;
	
	public override void Init()
	{
		base.Init();
		skillIndex = 1;
		skill = skillIndex switch
		{
			0 => new Nightingale_Sk_1(),
			1 => new Nightingale_Sk_2(),
			2 => new Nightingale_Sk_3(),
			_ => null
		};
		
		skill?.Init(this, skillIndex);
		
		if (skill?.SpChargeType == SpChargeType.PerSecond)
			GetComponent<OperatorController>().OnSkillSpCharge += skill.ChargeSP;
		
		ObjectPoolManager.Instance.CreatePoolDynamic(attackFX,3);
	}
	public override void Attack(HashSet<Unit> targetList)
	{
		foreach (var target in targetList)
		{
			if (target.Heal(GetFinalDamage()))
			{
				var fx = ObjectPoolManager.Instance.Spawn(attackFX.name);
				fx.transform.position = target.transform.position;
			}
		}
	}
	public override Skill GetSkill()
	{
		return skill;
	}

	public override void GetTargets(HashSet<Unit> targetList)
	{
		targetList.Clear();
		
		ITargetingTrait targeting = SubProfessionData as ITargetingTrait;
		targetList.AddRange(targeting.GetTargets(tilesInAttackRange));
	}
}