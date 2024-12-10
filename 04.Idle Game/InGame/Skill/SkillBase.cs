using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Unit = Unity.VisualScripting.Unit;

[System.Serializable]
public class SkillBase
{
	private float leftTime;
	public float CoolTime { get; protected set; }
	public bool CanUse { get; protected set; }
	public bool IsUsing { get; protected set; }

	protected float duration;
	protected float elapsedTime;

	public virtual void Init(Dictionary<string, object> data)
	{
		CoolTime = Convert.ToSingle(data["coolTime"]);
		duration = Convert.ToSingle(data["duration"]);
	}

	public void SetUseable()
	{
		elapsedTime = 0;
		leftTime = 0;
		CanUse = true;
	}

	public async virtual UniTaskVoid UseSkill()
	{
		CanUse = false;
		IsUsing = true;
	}

	public virtual void ApplyLevelUpValues(int level){}
	
	public void UpdateCoolTime(float deltaTime)
	{
		leftTime -= deltaTime;

		if (leftTime < 0)
		{
			leftTime = 0;
			CanUse = true;
		}
	}

	public void UpdateElapsedTime(float deltaTime)
	{
		elapsedTime += deltaTime;
	}

	protected void EndSkill()
	{
		IsUsing = false;
		leftTime = CoolTime;
		elapsedTime = 0;
	}

	public float GetCoolTimeVal() => leftTime / CoolTime;

	public float GetElapsedTime() => 1 - (elapsedTime / duration);

	public virtual string GetDescription(string description) { return null; }

}