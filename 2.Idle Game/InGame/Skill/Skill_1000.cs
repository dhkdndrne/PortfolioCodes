using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Bam.Extensions;
public class Skill_1000 : SkillBase
{
	private float damage;
	private float range;
	private float speed;
	private int amount;

	private float levelUpDmgVal;

	public override void Init(Dictionary<string, object> data)
	{
		base.Init(data);

		amount = Convert.ToInt32(data["value_1"]);
		damage = Convert.ToSingle(data["value_2"]);
		range = Convert.ToSingle(data["value_3"]);
		speed = Convert.ToSingle(data["value_4"]);

		levelUpDmgVal = Convert.ToSingle(data["value_2_up"]);
	}

	public async override UniTaskVoid UseSkill()
	{
		if (Player.Instance.PlayerUnit.GetTargetSqrDistance() > Extensions.Pow(range, 2))
			return;

		base.UseSkill().Forget();

		float tickVal = duration / amount;
		int amt = amount;

		while (amt > 0)
		{
			var bullet = ObjectPoolManager.Instance.Spawn("1000").GetComponent<Projectile>();
			bullet.transform.position = Player.Instance.PlayerUnit.transform.position;
			bullet.Init(Player.Instance.PlayerUnit,null,Player.Instance.GetTotalDamage() * (1 + (damage * 0.01f)), speed);

			await UniTask.Delay(TimeSpan.FromSeconds(tickVal));
			amt--;
		}

		EndSkill();
	}

	public override void ApplyLevelUpValues(int level)
	{
		damage += levelUpDmgVal * level;
	}

	public override string GetDescription(string description)
	{
		return description.Replace("{duration}", $"{duration}").Replace("{value_1}", $"{amount}").Replace("{value_2 * level}", $"{damage}");
	}
}