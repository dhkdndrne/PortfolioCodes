using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
[System.Serializable]
public class EnemySpawnSystem
{
	[SerializeField] private GameObject[] enemyPrefabList;
	
	private Dictionary<string, GameObject> enemyDic;

	public void Init()
	{
		enemyDic = new Dictionary<string, GameObject>();

		for (int i = 0; i < enemyPrefabList.Length; i++)
		{
			string id = enemyPrefabList[i].name.Split('_')[1];
			enemyDic[id] = enemyPrefabList[i];
		}
	}

	public void SpawnEnemy(List<string> enemyData)
	{
		for (int i = 0; i < enemyData.Count; i++)
		{
			var split = enemyData[i].Split('_');

			string key = split[0];
			int amount = int.Parse(split[1]);

			ObjectPoolManager.Instance.RegistNewObjectInPlay(key, enemyDic[key]);

			for (int j = 0; j < amount; j++)
			{
				var obj = ObjectPoolManager.Instance.Spawn(key);
				obj.transform.position = Player.Instance.PlayerUnit.transform.position + new Vector3(15 + Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);

				obj.GetComponent<Enemy>().SetID(key);
			
				var unit = obj.GetComponent<UnitAI>();
				unit.Init();
                
				StageManager.Instance.SpawnedEnemyList.Add(unit.MyHp);
			}
		}
	}
}