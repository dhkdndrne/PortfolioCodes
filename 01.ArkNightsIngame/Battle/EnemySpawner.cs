using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public class EnemySpawner
{
    [SerializeField] private Enemy testPrefab;
    [SerializeField] private GameObject trailPrefab;
    [SerializeField] private List<EnemyController> enemies = new List<EnemyController>();

    public int SpawnedEnemyCount => enemies.Count;
    
    public async UniTaskVoid Spawn(WayPointData wayPointData,WaveData wave)
    {	
        string enemyID = wave.enemyID;
        int order = wave.order;
        float interval = wave.interval;
        var poolManager = ObjectPoolManager.Instance;
        // todo 풀링 + 어드레서블 하면 다시 수정
        if (enemyID.Equals("Trail"))
        {
            var trailObj = GameObject.Instantiate(trailPrefab);
            trailObj.transform.position = wayPointData.points[0].Position;
            
            var trail = trailObj.GetComponent<TrailMove>();
            trail.SetWaypoint(wayPointData.points);
            trail.Move();
        }
        else
        {
            for (int i = 0; i < order; i++)
            {
                var obj = poolManager.Spawn(testPrefab.gameObject);
                obj.transform.position = wayPointData.points[0].Position;
                
                EnemyController enemy = obj.GetComponent<EnemyController>();
                enemies.Add(enemy);
                
                enemy.OnArrival += () =>
                {
                    enemies.Remove(enemy);
                    poolManager.DeSpawn(obj);
                };
                enemy.EnemyUnit.OnDeath += () =>
                {
                    enemies.Remove(enemy);
                    poolManager.DeSpawn(obj);
                };
             
                enemy.StartActive(wayPointData.points);
                
                float elapsed = 0f;
                while (elapsed < interval)
                {
                    elapsed += CustomTime.deltaTime;
                    await UniTask.Yield();
                }
            }
        }
    }
}
