using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoomerangProjectile : MonoBehaviour
{
   [SerializeField] private SpriteRenderer spr;
   private float duration = .5f;
   private Tweener rotationTween;
   
   public void Init(ColorLayer color)
   {
      spr.color = ColorManager.Instance.GetColor(color);
      Anim().Forget();
   }
   public async UniTaskVoid Anim()
   {
      // todo 보드에서 중복되지 않는 타겟 반환하도록 수정
      var target = SetTarget();
      
      rotationTween = transform.DORotate(new Vector3(0f, 0f, 360f), .1f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);

      var startPos = transform.position;
      Vector2 targetPos = target.transform.position;
      Vector3 controlPoint = GenerateRandomControlPoint(startPos, targetPos);

      float elapsedTime = 0f;

      while (elapsedTime < duration)
      {
         elapsedTime += Time.deltaTime;
         float t = elapsedTime / duration;
         Vector3 bezierPoint = CalculateBezierPoint(t, startPos, controlPoint, targetPos);
         transform.position = bezierPoint;

         // Optional: Adjust rotation to face the movement direction
         Vector3 direction = bezierPoint - transform.position;
         if (direction != Vector3.zero)
         {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, direction), Time.deltaTime * 10);
         }

         await UniTask.Yield();
      }

      // Ensure the final position is the target position
      transform.position = targetPos;
      //PopBlockDataManager.Instance.PopSet.Add(GameManager.Instance.Board.GetBlock(target.Hex));
      //Stage.Instance.Board.RemoveBlock(target.Hex);
      ObjectPoolManager.Instance.Despawn(GetComponent<PoolObject>());
   }
   Vector3 GenerateRandomControlPoint(Vector3 start, Vector3 end)
   {
      Vector3 midPoint = (start + end) / 2f;
      float randomOffset = Random.Range(-1f, 1f);
      Vector3 offset = new Vector3(randomOffset, randomOffset, 0f);
      return midPoint + offset;
   }
   private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
   {
      // Bezier formula: (1 - t)^2 * p0 + 2 * (1 - t) * t * p1 + t^2 * p2
      float u = 1 - t;
      float tt = t * t;
      float uu = u * u;

      Vector3 p = uu * p0; // (1 - t)^2 * p0
      p += 2 * u * t * p1; // 2 * (1 - t) * t * p1
      p += tt * p2;        // t^2 * p2

      return p;
   }
   private void OnDisable()
   {
      rotationTween?.Kill();
   }
   
   private Block SetTarget()
   {
      var board = GameManager.Instance.Board;

      int col = board.Col;
      int row = board.Row;

      Block target = null;
      while (target == null)
      {
         int x = Random.Range(0, col);
         int y = Random.Range(0, row);

         if (!board.IsValidIndex(x, y))
            continue;

         target = board.GetBlock(x, y);
      }

      return target;
   }
}
