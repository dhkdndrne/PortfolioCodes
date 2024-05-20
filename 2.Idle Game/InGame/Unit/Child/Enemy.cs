using System;
using UnityEngine;
using Random = UnityEngine.Random;
public class Enemy : UnitBase
{
	private PoolObject poolObj;
	private string id;
	
	private void Awake()
	{
		poolObj = GetComponent<PoolObject>();
	}

	private void OnDisable()
	{
		if (unitAI is not null)
			unitAI.MyHp.onDeathAction -= Dead;
	}

	public override void Init(UnitAI unitAI)
	{
		this.unitAI = unitAI;
		
		var data = DataManager.Instance.GetEnemyData;
		
		double hp = NumberTranslater.TranslateStringToDouble(data[id]["hp"].ToString());
		double damage = NumberTranslater.TranslateStringToDouble(data[id]["damage"].ToString());
		float atkSpeed = Convert.ToSingle(data[id]["atkSpeed"]);
		float moveSpeed = Convert.ToSingle(data[id]["moveSpeed"]);
		float attackRange = Convert.ToSingle(data[id]["atkRange"]);
		float criRate = Convert.ToSingle(data[id]["criRate"]);
		float criDamage = Convert.ToSingle(data[id]["criDamage"]);
        
		unitAI.MyHp.Init(hp);
		unitAI.MyHp.onDeathAction += Dead;
        
		Stat = new UnitStat(damage, attackRange, atkSpeed, moveSpeed, criDamage,criRate);
		isAttack = false;
	}

	public void SetID(string id) => this.id = id;
	
	protected override void OnAttack()
	{
		unitAI.Target.Hit(Stat.Damage);
	}

	protected override void EndAttack()
	{
		isAttack = false;
		SetCoolTime();
	}
	protected override void Dead()
	{
		Player.Instance.Currency.IncreaseGold(StageManager.Instance.CurStage.GetReward());
		
		if(!LowBatterySystem.Instance.IsLowBatterMode)
			SpawnBone();

		StageManager.Instance.SpawnedEnemyList.Remove(unitAI.MyHp);
		ObjectPoolManager.Instance.Despawn(poolObj);
	}

	private void SpawnBone()
	{
		int num = Random.Range(3, 8);
		for (int i = 0; i < num; i++)
		{
			var obj = ObjectPoolManager.Instance.Spawn("Bone_1");
			obj.transform.position = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1f, 2f), 0);
		}
		var obj2 = ObjectPoolManager.Instance.Spawn("Bone_2");
		obj2.transform.position = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1f, 2f), 0);
	}
}