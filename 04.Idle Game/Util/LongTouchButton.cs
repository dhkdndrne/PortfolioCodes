using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LongTouchButton : MonoBehaviour
{
	private Button button;
	private bool isLongClick;

	private void Awake()
	{
		button = GetComponent<Button>();
	}

	public void Init(float interval, float longClickTimer, Action action)
	{
		// 버튼을 누를 때 Observable을 생성
		var buttonDownStream = button.OnPointerDownAsObservable();

		// 버튼을 떼었을 때 Observable을 생성
		var buttonUpStream = button.OnPointerUpAsObservable();

		buttonDownStream
			.SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(longClickTimer)))
			.Where(_ => !isLongClick)
			.TakeUntil(buttonUpStream)
			.RepeatUntilDestroy(gameObject)
			.Subscribe(_ => isLongClick = true);

		buttonDownStream
			.SelectMany(_ => Observable.EveryUpdate().TakeUntil(buttonUpStream))
			.Where(_ => isLongClick)
			.ThrottleFirst(TimeSpan.FromSeconds(interval)) // 일정한 시간 간격으로 작업 실행
			.Subscribe(_ =>
			{
				action?.Invoke();
				PlayBounceAnim(interval).Forget();
			});


		buttonUpStream.Timestamp()
			.Zip(buttonDownStream.Timestamp(), (u, d) => (u.Timestamp - d.Timestamp).TotalMilliseconds / 1000.0f)
			.Where(time => time < 1.0f)
			.Subscribe(_ =>
			{
				action?.Invoke(); // 다른 동작 실행
				PlayBounceAnim(interval).Forget();
			}).AddTo(this);

		buttonUpStream.Subscribe(_ =>
		{
			isLongClick = false;
		}).AddTo(this);
	}

	private async UniTaskVoid PlayBounceAnim(float interval)
	{
		await transform.DOScale(Vector3.one * 1.05f,interval * 10).SetEase(Ease.OutBounce);
		await transform.DOScale(Vector3.one,interval);
	}
}