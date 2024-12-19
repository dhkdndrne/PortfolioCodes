using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SpawnSystem
{
   public void SpawnEnemy()
   {
      SpawnAsync().Forget();
   }

   private async UniTaskVoid SpawnAsync()
   {
      for (int i = 0; i < 2; i++)
      {
         var monObj = ObjectPoolManager.Instance.Spawn("slime");
         monObj.transform.position = Vector3.zero;
         monObj.GetComponent<EnemyAI>().Init();

         await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
      }
   }
}
