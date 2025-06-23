using System.Collections;
using Bam.Singleton;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// 페이드 효과를 관리하는 매니저 클래스
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class FadeManager : DontDestroySingleton<FadeManager>
{
	private CanvasGroup fadeCanvasGroup;
	[SerializeField] private float duration = 1f;

	protected override void Init()
	{
		fadeCanvasGroup = GetComponent<CanvasGroup>();
	}

	public async UniTask FadeAsync(UniTask onFadeState, float duration = 1f, UniTask onBegin = default, UniTask onEnd = default)
	{
		await onBegin;
		fadeCanvasGroup.gameObject.SetActive(true);
        
		// 페이드 인
		await FadeIn(duration);
        
		// 중간 상태 처리
		await onFadeState;

		// 페이드 아웃
		await FadeOut(duration);

		fadeCanvasGroup.gameObject.SetActive(false);
		await onEnd;
	}

	/// <summary>
	/// 페이드 인
	/// </summary>
	public async UniTask FadeIn(float duration = 1f)
	{
		fadeCanvasGroup.alpha = 0f;
		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
			await UniTask.Yield();
		}
		fadeCanvasGroup.alpha = 1f;
	}

	/// <summary>
	/// 페이드 아웃
	/// </summary>
	public async UniTask FadeOut(float duration = 1f)
	{
		fadeCanvasGroup.alpha = 1f;
		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
			await UniTask.Yield();
		}
		fadeCanvasGroup.alpha = 0f;
	}
}