using System;
using System.Text;
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
		/// <param name="array"></param>
		/// <param name="isLeft"></param>
		/// <returns></returns>
		public static T[,] RotateArray<T>(T[,] array, int angle)
		{
			int r = array.GetLength(0);                      // 원본 행 수
			int c = array.GetLength(1);                      // 원본 열 수
			int numRotations = ((angle % 360) / 90 + 4) % 4; // 0, 1, 2, 3 중 하나

			// 0도, 180도는 원본과 같은 크기, 90도, 270도는 행과 열이 뒤바뀜
			int newRows = (numRotations % 2 == 0) ? r : c;
			int newCols = (numRotations % 2 == 0) ? c : r;
			T[,] result = new T[newRows, newCols];

			for (int i = 0; i < r; i++)
			{
				for (int j = 0; j < c; j++)
				{
					switch (numRotations)
					{
						case 0:
							// 0도 회전: 그대로 복사
							result[i, j] = array[i, j];
							break;
						case 1:
							// 270도 회전 시: result[c - 1 - j, i] = array[i, j]
							result[c - 1 - j, i] = array[i, j];
							break;
						case 2:
							// 180도 회전 시: result[r - 1 - i, c - 1 - j] = array[i, j]
							result[r - 1 - i, c - 1 - j] = array[i, j];
							break;
						case 3:
							// 90도 회전 시: result[j, r - 1 - i] = array[i, j]
							result[j, r - 1 - i] = array[i, j];
							break;
					}
				}
			}
			return result;
		}


		/// 2차원 인덱스를 1차원 인덱스로 변환
		public static int Get1DIndex( int x, int y, int col) => y * col + x;
		
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

		public static bool IsPointerOverObject() => !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

        #endregion
		
        #region Animator

		// 현재 진행 중인 애니메이션의 이름이 지정된 이름과 같은지 확인
		public static bool IsCurrentAnimation(this Animator animator, string animationName)
		{
			return animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
		}
		
		/// <summary>
		/// 애니메이터에 있는 클립 중 지정된 이름과 같은 애니메이션이 있으면
		/// speedmuliplier가 적용된 실제 속도 반환
		/// </summary>
		/// <param name="animator"></param>
		/// <param name="animationName"></param>
		/// <param name="speedScaleHash"></param>
		/// <returns></returns>
		public static float GetAnimationActualLength(this Animator animator, string animationName, int speedParameterHash)
		{
			var clips = animator.runtimeAnimatorController.animationClips;
			foreach (var clip in clips)
			{
				if (clip.name.Equals(animationName))
				{
					float baseDuration = clip.length;
					float speedMultiplier = animator.GetFloat(speedParameterHash);

					if (speedMultiplier == 0)
					{
						Debug.LogWarning($"애니메이터 파마미터 speedMultiplier가 0임 '{animationName}'");
						return baseDuration; // fallback
					}

					return baseDuration / speedMultiplier;
				}
			}

			Debug.LogWarning($"'{animationName}'가 애니메이터에 없다");
			return 0;
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

        #endregion

		#region MPB

		public static void ChangeMeshColor(MeshRenderer meshRenderer, MaterialPropertyBlock mpb, Color color, string shaderColorKey = "_BaseColor")
		{
			meshRenderer.GetPropertyBlock(mpb);
			mpb.SetColor(shaderColorKey, color);
			meshRenderer.SetPropertyBlock(mpb);
		}
		
		public static void ChangeMeshColor(SkinnedMeshRenderer skinnedMeshRenderer, MaterialPropertyBlock mpb, Color color, string shaderColorKey = "_BaseColor")
		{
			skinnedMeshRenderer.GetPropertyBlock(mpb);
			mpb.SetColor(shaderColorKey, color);
			skinnedMeshRenderer.SetPropertyBlock(mpb);
		}
		#endregion

		public static void SetActive(this Component target, bool active)
		{
			target.gameObject.SetActive(active);
		}

		public static T ToEnum<T>(this string value) where T : Enum
		{
			if (string.IsNullOrEmpty(value))
				return (T)Enum.GetValues(typeof(T)).GetValue(0);

			return (T)Enum.Parse(typeof(T), value);
		}
		
		public static float Distance(Vector3 a, Vector3 b)
		{
			float num1 = a.x - b.x;
			float num3 = a.z - b.z;
			return Mathf.Sqrt(num1 * num1 +num3 * num3);
		}
		
		/// <summary>
		/// 2개의 observablevalue 변수를 동해 결과를 도출할 필요가 있을때 사용
		/// ex) cost와 배치 포인트가 둘다 만족하는지 체크할때
		/// </summary>
		/// <param name="source1"></param>
		/// <param name="source2"></param>
		/// <param name="combine"></param>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <returns></returns>
		public static ObservableValue<TResult> CombineLatest<T1, T2, TResult>(
			ObservableValue<T1> source1,
			ObservableValue<T2> source2,
			Func<T1, T2, TResult> combine)
		{
			// 초기 값은 두 소스의 현재 값을 결합한 결과로 설정
			var combined = new ObservableValue<TResult>(combine(source1.Value, source2.Value));
        
			// source1의 값이 변경될 때마다 결합 결과를 업데이트
			source1.Subscribe(val1 => 
			{
				combined.Value = combine(val1, source2.Value);
			});
        
			// source2의 값이 변경될 때마다 결합 결과를 업데이트
			source2.Subscribe(val2 =>
			{
				combined.Value = combine(source1.Value, val2);
			});
        
			return combined;
		}
	}
}