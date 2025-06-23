using Bam.Singleton;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : DontDestroySingleton<FadeManager>
{
    #region Inspector

	[SerializeField] private CanvasGroup fadeCanvasGroup;                           
	[SerializeField] private int sortingOrder = 100;                               
	[SerializeField] private Vector2 referenceResolution = new Vector2(2960, 1440);
	
    #endregion

	protected override void Awake()
	{
		base.Awake();

		if (fadeCanvasGroup != null)
		{
			fadeCanvasGroup.alpha = 0;
			fadeCanvasGroup.gameObject.SetActive(false);
		}
	}

	public async UniTask FadeAsync(UniTask onFadeState,float duration = 1f, UniTask onBegin = default, UniTask onEnd = default)
	{
		await onBegin;
		fadeCanvasGroup.gameObject.SetActive(true);
		await fadeCanvasGroup.DOFade(1f, duration).SetUpdate(true).AsyncWaitForCompletion();
		await onFadeState;
		await fadeCanvasGroup.DOFade(0f, duration).SetUpdate(true).AsyncWaitForCompletion();
		fadeCanvasGroup.gameObject.SetActive(false);
		await onEnd;
	}
	/// <summary>
	/// 페이드 인
	/// </summary>
	/// <param name="duration"></param>
	public async UniTask FadeIn(float duration = 1f)
	{
		fadeCanvasGroup.gameObject.SetActive(true);
		fadeCanvasGroup.alpha = 1f;
		
		await UniTask.Delay(350);
		await fadeCanvasGroup.DOFade(0f, duration).SetUpdate(true).AsyncWaitForCompletion();
		fadeCanvasGroup.gameObject.SetActive(false);
	}
	
    #region Editor Only

#if UNITY_EDITOR
	[ContextMenu("FadeCanvas 자동 생성")]
	private void CreateFadeCanvas()
	{
		if (IsExistFadeCanvas()) return;

		GameObject fadeCanvasObj = null;
		if (!fadeCanvasGroup) fadeCanvasObj = new GameObject("FadeCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(CanvasGroup));
		else fadeCanvasObj = fadeCanvasGroup.gameObject;
	
		Canvas fadeCanvas = fadeCanvasObj.GetComponent<Canvas>();
		CanvasScaler fadeCanvasScaler = fadeCanvasObj.GetComponent<CanvasScaler>();
		fadeCanvasGroup = fadeCanvasObj.GetComponent<CanvasGroup>();
		if (!fadeCanvas) fadeCanvas = fadeCanvasObj.AddComponent<Canvas>();
		if (!fadeCanvasScaler) fadeCanvasScaler = fadeCanvasObj.AddComponent<CanvasScaler>();
		if (!fadeCanvasGroup) fadeCanvasGroup = fadeCanvasObj.AddComponent<CanvasGroup>();

		fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
		fadeCanvas.sortingOrder = sortingOrder;
		fadeCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		fadeCanvasScaler.referenceResolution = referenceResolution;
		fadeCanvasGroup.blocksRaycasts = true;
	}

	private bool IsExistFadeCanvas()
	{
		if (!fadeCanvasGroup || !fadeCanvasGroup.blocksRaycasts) return false;
		Canvas fadeCanvas = fadeCanvasGroup.GetComponent<Canvas>();
		if (!fadeCanvas || fadeCanvas.renderMode != RenderMode.ScreenSpaceOverlay || fadeCanvas.sortingOrder != sortingOrder) return false;
		CanvasScaler fadeCanvasScaler = fadeCanvasGroup.GetComponent<CanvasScaler>();
		if (!fadeCanvasScaler || fadeCanvasScaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize || fadeCanvasScaler.referenceResolution != referenceResolution) return false;
		return true;
	}
#endif

    #endregion
}