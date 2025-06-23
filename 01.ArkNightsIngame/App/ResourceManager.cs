using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

public class ResourceManager
{
   private Dictionary<string,UnityEngine.Object> resources = new Dictionary<string,UnityEngine.Object>();

   public void LoadAsync<T>(string key, Action<T> callback) where T : UnityEngine.Object
   {
      //캐시 확인
      if (resources.TryGetValue(key, out Object resource))
      {
         callback?.Invoke(resource as T);
         return;
      }

      // 리소스 비동기 로딩
      var asyncOp = Addressables.LoadAssetAsync<T>(key);
      asyncOp.Completed += op =>
      {
         resources[key] = op.Result;
         callback?.Invoke(op.Result);
      };
   }

   public void LoadAllAsync<T>(string label, Action<string,int,int> callback) where T : UnityEngine.Object
   {
      var ops = Addressables.LoadResourceLocationsAsync(label, typeof(T));

      ops.Completed += op =>
      {
         int loadCnt = 0;
         int totalCnt = op.Result.Count;

         foreach (var result in op.Result)
         {
            LoadAsync<T>(result.PrimaryKey, obj =>
            {
               loadCnt++;
               callback?.Invoke(result.PrimaryKey, loadCnt, totalCnt);
            });
         }
      };
   }

   public void Destroy(GameObject obj)
   {
      if (obj == null)
         return;

      ObjectPoolManager.Instance.DeSpawn(obj);
   }
}
