using System.Collections.Generic;
using System.Linq;
using Bam.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SilverAsh_Sk_1 : Skill
{
	private float damageRatio;
	private string animationClipName;
	
	protected override void PostInit(Operator op, int index)
	{
		animationClipName = $"Skill_{index + 1}";
		var data = DataManager.Instance.OperatorData.GetOperatorSkillData(id);
		damageRatio = data.GetSkillLevelData(10).Attributes[0].Value;
	}
	
	public override void SetSkill()
	{
		isEnd = false;
		isActivating = true;
		
		duration = op.Controller.GetAnimActualLength(animationClipName);
		elapsedTime = duration;

		base.SetSkill();
	}

	private async UniTaskVoid RunDurationTimer()
	{
		while (elapsedTime > 0)
		{
			elapsedTime -= Time.deltaTime;
			DurationRatio.Value = elapsedTime /duration;
			await UniTask.Yield();
		}
	}
	public override bool CanUse()
	{
		return base.CanUse() && op.Controller.AtkCoolTime <= 0;
	}
	public override bool IsSkillEnd()
	{
		if (isEnd)
		{
			op.Controller.SetAtkCoolTime();
			Sp.Value = 0;
		}
		return isEnd;
	}
	public override bool UpdateSkill(HashSet<Unit> targetList)
	{
		if (targetList.Count > 0 && isActivating)
		{
			isActivating = false;
			RunDurationTimer();
			return true;
		}
		return false;
	}

	public override void Use(Operator caster, HashSet<Unit> targetList)
	{
		float damage = caster.GetFinalDamage() * damageRatio;

		Debug.Log("스킬");
		var target = targetList.FirstOrDefault();
		target?.Hit(caster);

		isEnd = true;
	}
}