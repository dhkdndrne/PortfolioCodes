using System;

/// <summary>
/// UniRx ReactiveProperty처럼 사용하려고 만든 커스텀 클래스
/// </summary>
/// <typeparam name="T"></typeparam>
public class ObservableValue<T>
{
	public event Action<T> OnValueChanged; // 값이 변경될 때 호출될 이벤트
	private T value;

	public T Value
	{
		get => value;
		set
		{
			// 값이 변할 때만 실행 (null 체크 등 필요 시 보완)
			if (!this.value.Equals(value))  
			{
				this.value = value;
				OnValueChanged?.Invoke(this.value);
			}
		}
	}
	
	public ObservableValue(T initialValue)
	{
		value = initialValue;
	}

	/// <summary>
	/// 구독: 초기값을 발행한 후 이벤트에 observer 추가하고, IDisposable을 반환하여 나중에 구독 해제가 가능하도록 함
	/// </summary>
	/// <param name="observer">구독할 액션</param>
	/// <returns>IDisposable을 반환하여 Dispose 시 구독 해제</returns>
	public IDisposable Subscribe(Action<T> observer)
	{
		// 구독 시 초기값 전달
		observer(value);
		// 이벤트에 등록
		OnValueChanged += observer;
		// 구독 해제를 위한 IDisposable 반환
		return new Subscription(() => Unsubscribe(observer));
	}

	/// <summary>
	/// 구독 해제
	/// </summary>
	/// <param name="observer">해제할 액션</param>
	public void Unsubscribe(Action<T> observer)
	{
		OnValueChanged -= observer;
	}

	/// <summary>
	/// IDisposable 구현 내부 클래스 (구독 해제 로직을 래핑)
	/// </summary>
	private class Subscription : IDisposable
	{
		private readonly Action disposeAction;
		private bool disposed;

		public Subscription(Action disposeAction)
		{
			this.disposeAction = disposeAction;
		}

		public void Dispose()
		{
			if (!disposed)
			{
				disposeAction?.Invoke();
				disposed = true;
			}
		}
	}
}