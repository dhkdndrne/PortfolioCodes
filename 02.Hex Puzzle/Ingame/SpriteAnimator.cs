using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
   [SerializeField] private SpriteRenderer spriteRenderer;
   [SerializeField] private bool isLoop;
   [SerializeField] private Sprite[] sprites;
   [SerializeField] private float duration;

   private PoolObject poolObject;
   private void Awake()
   {
      poolObject = GetComponent<PoolObject>();
   }

   public async UniTask StartAnimation()
   {
      if (sprites.Length == 0)
         return;
      
      float spritePerTime = duration / sprites.Length;
      float elapsedTime = 0;
      int index = 0;
      spriteRenderer.sprite = sprites[index];
      
      while (true)
      {
         if (elapsedTime >= spritePerTime)
         {
            spriteRenderer.sprite = sprites[++index];
            elapsedTime = 0;
         }

         if (index == sprites.Length - 1)
         {
            if (!isLoop)
               break;
            
            index = 0;
         }
         
         await UniTask.Yield();
         elapsedTime += Time.deltaTime;
      }
      
      ObjectPoolManager.Instance.Despawn(poolObject);
   }
}
