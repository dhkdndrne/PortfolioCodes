using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bam.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Skill_1002 : SkillBase
{
	private double damage;
	private float range;
	private float levelUpDmgVal;

	public override void Init(Dictionary<string, object> data)
	{
		base.Init(data);

		damage = NumberTranslater.TranslateStringToDouble(data["value_1"].ToString());
		range = Convert.ToSingle(data["value_2"]);
		levelUpDmgVal = Convert.ToSingle(data["value_1_up"]);
	}

	public async override UniTaskVoid UseSkill()
	{
		if (Player.Instance.PlayerUnit.GetTargetSqrDistance() > Extensions.Pow(range, 2))
			return;

		base.UseSkill().Forget();

		var fxObject = ObjectPoolManager.Instance.Spawn("1002");
		
		Transform transform;
		(transform = fxObject.transform).SetParent(Player.Instance.PlayerUnit.transform);
		transform.localPosition = Vector3.zero;

		var list = StageManager.Instance.SpawnedEnemyList
			.Where(enemy => enemy != null && Vector3.Distance(enemy.transform.position,Player.Instance.PlayerUnit.transform.position) <= range).ToList();

		foreach (var enemy in list)
		{
			enemy.Hit(damage);
		}
		
		await UniTask.Delay(1000);
		EndSkill();
	}

	public override void ApplyLevelUpValues(int level)
	{
		damage += levelUpDmgVal * level;
	}
	public override string GetDescription(string description)
	{
		return description.Replace("{value_1}", $"{damage}");
	}
}