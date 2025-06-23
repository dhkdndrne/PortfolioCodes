#if UNIRX_SUPPORT
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Bam.Extensions
{
	public static class Extensions
	{
        #region Transform

		public static void SetPosX(this Transform transform, float xValue)
		{
			var tempPoision = transform.position;
			tempPoision = new Vector3(xValue, tempPoision.y, tempPoision.z);
			transform.position = tempPoision;
		}

		public static void SetPosY(this Transform transform, float yValue)
		{
			var tempPoision = transform.position;
			tempPoision = new Vector3(tempPoision.x, yValue, tempPoision.z);
			transform.position = tempPoision;
		}

		public static void SetPosZ(this Transform transform, float zValue)
		{
			var tempPoision = transform.position;
			tempPoision = new Vector3(tempPoision.x, tempPoision.y, zValue);
			transform.position = tempPoision;
		}

		public static void InitTransform(this Transform transform)
		{
			transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
			transform.localScale = Vector3.one;
		}

		public static void InitTransform(this Transform transform, Vector3 position, float scale)
		{
			transform.SetPositionAndRotation(position, Quaternion.identity);
			transform.localScale = Vector3.one * scale;
		}

		public static void InitTransform(this Transform transform, Vector3 position, Quaternion quaternion)
		{
			transform.SetPositionAndRotation(position, quaternion);
			transform.localScale = Vector3.one;
		}

		public static void InitLocalTransform(this Transform transform)
		{
			transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			transform.localScale = Vector3.one;
		}

		public static void InitLocalTransform(this Transform transform, Vector3 position, Quaternion quaternion, float scale)
		{
			transform.SetLocalPositionAndRotation(position, quaternion);
			transform.localScale = Vector3.one * scale;
		}

		public static void InitLocalTransform(this Transform transform, Vector3 position, Quaternion quaternion)
		{
			transform.SetLocalPositionAndRotation(position, quaternion);
			transform.localScale = Vector3.one;
		}

        #endregion

        #region Math

		/// <summary>
		/// 절대값 계산 커스텀
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static int Abs(int value)
		{
			return value > 0 ? value : -value;
		}

		/// <param name="a">밑</param>
		/// <param name="b">지수</param>
		/// <returns>밑을 지수곱만큼 제곱한 결괏값(int)</returns>
		public static int Pow(int a, int b)
		{
			int y = 1;
			while (true)
			{
				if ((b & 1) != 0) y = a * y;
				b = b >> 1;
				if (b == 0) return y;
				a *= a;
			}
		}
		public static float Pow(float a, int b)
		{
			float y = 1;
			while (true)
			{
				if ((b & 1) != 0) y = a * y;
				b = b >> 1;
				if (b == 0) return y;
				a *= a;
			}
		}

		//2d
		public static float VectorToAngle(Vector2 direction)
		{
			return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
		}

		public static Vector2 AngleToVector(float angle)
		{
			float radianAngle = angle * Mathf.Deg2Rad;
			return new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle));
		}

		public static Vector2 RotateVectorByAngle(Vector2 vector, float angle)
		{
			Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
			Vector2 rotatedVector = rotation * vector;
			return rotatedVector;
		}
		/// <summary>
		/// 두 원이 겹쳤는지 체크
		/// </summary>
		/// <param name="circle1"></param>
		/// <param name="circle2"></param>
		/// <param name="radius1"></param>
		/// <param name="radius2"></param>
		/// <returns></returns>
		public static bool CheckCircleCollision(Vector2 circle1, Vector2 circle2, float radius1, float radius2)
		{
			return Vector3.SqrMagnitude(circle1 - circle2) <= Pow(radius1 + radius2, 2);
		}

		/// <summary>
		/// 타겟이 화면 안에 있는지 
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public static bool CheckTargetInScreen(Vector2 position)
		{
			Vector3 screenPoint = Camera.main.WorldToViewportPoint(position);
			return screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1 && screenPoint.z > 0;
		}

		/// <summary>
		/// 행렬 회전
		/// </summary>
		/// <param name="mat"></param>
		/// <param name="isLeft"></param>
		/// <returns></returns>
		public static int[,] RotateArray(int[,] mat, bool isLeft = false)
		{
			int lengthY = mat.GetLength(0);
			int lengthX = mat.GetLength(1);
			int[,] result = new int[lengthX, lengthY];

			for (int y = 0; y < lengthY; y++)
			{
				for (int x = 0; x < lengthX; x++)
				{
					result[x, y] = isLeft ? mat[lengthY - 1 - y, x] : mat[y, lengthX - 1 - x];
				}
			}

			return result;
		}

		/// <summary>
		/// 숫자 n퍼센트 증가
		/// </summary>
		/// <returns></returns>
		public static float IncreasePercent(float num, float percent)
		{
			return num * (1 + percent * 0.01f);
		}

		/// <summary>
		/// 숫자 n퍼센트 감소
		/// </summary>
		/// <returns></returns>
		public static float DecreasePercent(float num, float percent)
		{
			return num * (1 - percent * 0.01f);
		}
		
        #endregion

        #region Ray
		public static bool IsPointerOverUI() => UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
		public static Ray ScreenPointToRay() => Camera.main.ScreenPointToRay(Input.mousePosition);
		public static Vector3 ScreenToWorldPoint(Vector3 position) => Camera.main.ScreenToWorldPoint(position);
		public static Vector3 ScreenToViewportPoint(Vector3 position) => Camera.main.ScreenToViewportPoint(position);

        #endregion

        #region UniRx

#if UNIRX_SUPPORT
        // 현재 진행 중인 애니메이션이 종료 시 이벤트 호출하는 기능을 UniRx에 추가
        public static IObservable<Unit> OnAnimationCompleteAsObservable(this Animator animator)
        {
            return Observable.EveryUpdate().Where(_ => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f - animator.GetAnimatorTransitionInfo(0).duration).AsUnitObservable().FirstOrDefault();
        }

        // 지정된 애니메이션이 종료 시 이벤트 호출하는 기능을 UniRx에 추가
        public static IObservable<Unit> OnAnimationCompleteAsObservable(this Animator animator, string animationName)
        {
            return Observable.EveryUpdate().Where(_ => animator.GetCurrentAnimatorStateInfo(0).IsName(animationName)).Where(_ => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f - animator.GetAnimatorTransitionInfo(0).duration)
                .AsUnitObservable().FirstOrDefault();
        }

        // 지정된 애니메이션이 종료 시 이벤트 호출하는 기능을 UniRx에 추가
        public static IObservable<Unit> OnAnimationCompleteAsObservable(this Animator animator, int animationHash)
        {
            return Observable.EveryUpdate().Where(_ => animator.GetCurrentAnimatorStateInfo(0).shortNameHash == animationHash).Where(_ => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f - animator.GetAnimatorTransitionInfo(0).duration)
                .AsUnitObservable().FirstOrDefault();
        }
#endif

        #endregion

        #region UniTask

#if UNIRX_SUPPORT
        // 지정된 애니메이션이 종료 시까지 대기하는 기능을 UniTask에 추가
        public static async UniTask WaitAnimationCompleteAsync(this Animator animator, string animationName)
        {
            await animator.OnAnimationCompleteAsObservable(animationName).ToUniTask(cancellationToken: animator.GetCancellationTokenOnDestroy());
        }
        
        // 지정된 애니메이션이 종료 시까지 대기하는 기능을 UniTask에 추가
        public static async UniTask WaitAnimationCompleteAsync(this Animator animator, int animationHash)
        {
            await animator.OnAnimationCompleteAsObservable(animationHash).ToUniTask(cancellationToken: animator.GetCancellationTokenOnDestroy());
        }
        
        // 현재 진행 중인 애니메이션이 종료 시까지 대기하는 기능을 UniTask에 추가
        public static async UniTask WaitAnimationCompleteAsync(this Animator animator)
        {
            await animator.OnAnimationCompleteAsObservable().ToUniTask(cancellationToken: animator.GetCancellationTokenOnDestroy());
        }
                
        // 현재 진행 중인 애니메이션이 종료 시까지 대기하는 기능을 UniTask에 추가 (토큰 설정)
        public static async UniTask WaitAnimationCompleteAsync(this Animator animator,CancellationToken token)
        {
            await animator.OnAnimationCompleteAsObservable().ToUniTask(cancellationToken: token);
        }
        // 파티클 재생 종료 시까지 대기하는 기능을 UniTask에 추가
        public static async UniTask WaitParticleCompleteAsync(this ParticleSystem particleSystem)
        {
            await UniTask.WaitWhile(() => particleSystem.IsAlive(), cancellationToken: particleSystem.GetCancellationTokenOnDestroy());
        }
#endif

        #endregion

        #region Animator

		// 현재 진행 중인 애니메이션의 이름이 지정된 이름과 같은지 확인
		public static bool IsCurrentAnimation(this Animator self, string animationName)
		{
			return self.GetCurrentAnimatorStateInfo(0).IsName(animationName);
		}

        #endregion

        #region String

		/// <summary>
		/// 문자열이 null이거나 빈칸인지 체크
		/// </summary>
		/// <param name="str"></param>
		public static bool IsNullOrWhitespace(this string str)
		{
			if (string.IsNullOrEmpty(str)) return true;

			for (int i = 0; i < str.Length; i++)
			{
				if (!char.IsWhiteSpace(str[i]))
					return false;
			}
			return true;
		}

        #endregion

		#region MPB

		public static void ChangeMeshColor(MeshRenderer meshRenderer,MaterialPropertyBlock mpb, Color color,string shaderKey)
		{
			meshRenderer.GetPropertyBlock(mpb);
			mpb.SetColor(shaderKey, color);
			meshRenderer.SetPropertyBlock(mpb);
		}

		#endregion

		private static StringBuilder sb = new StringBuilder();
		public static string GetTextAppendLine(params string[] s)
		{
			sb.Clear();

			for (int i = 0; i < s.Length; i++)
			{
				sb.Append(s[i]);
			}

			return sb.ToString();
		}
	}
}