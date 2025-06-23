using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyData
{
	private const string URL_EnemyData = "https://docs.google.com/spreadsheets/d/1bV3EVBbS1NfVahKsUT4W1dVwksKTrQxQOjGhkrrAgAA/export?format=tsv&gid=1387133437";
	private Dictionary<int, EnemyBaseData> dic = new();

	public async UniTask LoadData()
	{
		await LoadEnemyData();
	}
	public EnemyBaseData GetEnemyData(int id)
	{
		return dic.ContainsKey(id) ? dic[id] : null;
	}
	
	private async UniTask LoadEnemyData()
	{
		var data = await SpreadSheetReader.LoadGoogleSheet(URL_EnemyData);
		
		
		for (int i = 1; i < data.Length; i++)
		{
			string[] column = data[i].Split("\t");
			
			int id = int.Parse(column[0]);
			
			float hp = float.Parse(column[1]);
			float moveSpeed = float.Parse(column[2]);
			float atkPower = float.Parse(column[3]);
			float defense = float.Parse(column[4]);
			float magicDefense = float.Parse(column[5]);
			float atkCoolTime = float.Parse(column[6]);
				
			dic[id] = new EnemyBaseData(hp,moveSpeed,atkPower,defense,magicDefense,atkCoolTime);
		}
	}
}

public class EnemyBaseData
{
	private float hp;
	private float moveSpeed;
	private float attackPower;
	private float defense;
	private float magicDefense;
	private float attackCoolTime;

	public float Hp => hp;
	public float MoveSpeed => moveSpeed;
	public float AttackPower => attackPower;
	public float Defense => defense;
	public float MagicDefense => magicDefense;
	public float AttackCoolTime => attackCoolTime;

	public EnemyBaseData(float hp, float moveSpeed, float attackPower, float defense, float magicDefense, float attackCoolTime)
	{
		this.hp = hp;
		this.moveSpeed = moveSpeed;
		this.attackPower = attackPower;
		this.defense = defense;
		this.magicDefense = magicDefense;
		this.attackCoolTime = attackCoolTime;
	}
}