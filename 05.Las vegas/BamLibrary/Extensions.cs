#if UNIRX_SUPPORT
using UniRx;
using UniRx.Triggers;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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

        public static void InitTransform(this Transform transform,Vector3 position , float scale)
        {
            transform.SetPositionAndRotation(position, Quaternion.identity);
            transform.localScale = Vector3.one * scale;
        }
        
        public static void InitTransform(this Transform transform,Vector3 position,Quaternion quaternion)
        {
            transform.SetPositionAndRotation(position,quaternion);
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

        public static float Abs(float value)
        {
            return value > 0 ? value : -value;
        }

        /// <param name="a">밑</param>
        /// <param name="b">지수</param>
        /// <returns>밑을 지수곱만큼 제곱한 결괏값(int)</returns>
        public static int Pow(int a, uint b)
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
        public static float Pow(float a, uint b)
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
    }
}