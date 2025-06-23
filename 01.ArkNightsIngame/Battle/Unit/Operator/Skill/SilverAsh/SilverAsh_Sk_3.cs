using System.Collections.Generic;
using Bam.Extensions;
using UnityEngine;

public class SilverAsh_Sk_3 : Skill, IRangeModifyingSkill
{
	private float interval;
	private bool isAttacking;
	private int maxTarget;
	private ParticleSystem fx;
	
	public int RangeID
	{
		get;
		private set;
	}

	protected override void PostInit(Operator op, int index)
	{
		fx = op.transform.Find("FX_SwordWaveWhite").GetComponent<ParticleSystem>();
		fx.SetActive(true);
	}
	
	public override void SetSkill()
	{
		var data = DataManager.Instance.OperatorData.GetOperatorSkillData(id).GetSkillLevelData(10);
		
		isActivating = true;
		isAttacking = false;
		interval = 1f;
		duration = (int)data.Duration;
		elapsedTime = duration;
		maxTarget = (int)data.Attributes[2].Value;
		RangeID = data.RangeID;
		
		float bodyYRotation = op.Controller.Body.eulerAngles.y - 90;
		fx.transform.rotation = Quaternion.Euler(-90f, bodyYRotation, 0f);
		
		base.SetSkill();
	}

	public override bool UpdateSkill(HashSet<Unit> targetList)
	{
		if (elapsedTime <= 0)
		{
			isEnd = true;
			isActivating = false;
			isAttacking = false;
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

	public override void Use(Operator caster, HashSet<Unit> targetList)
	{
		int cnt = 0;
		fx.Play();
		foreach (var target in targetList)
		{
			target.Hit(caster);
			cnt++;
			if (cnt > maxTarget)
				break;
		}
	}

	public override bool IsSkillEnd()
	{
		return isEnd;
	}

	public override void EndSkillAnim()
	{
		interval = 1f;
		isAttacking = false;
	}
	public GridType[,] GetModifiedAttackRange()
	{
		return DataManager.Instance.RangeDic[RangeID].GetGridArray();
	}

}