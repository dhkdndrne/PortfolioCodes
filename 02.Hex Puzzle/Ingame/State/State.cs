using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(StateMachine))]
public abstract class State : MonoBehaviour
{
	private StateMachine machine;
	//스테이트 개시
	private Subject<Unit> begin = new Subject<Unit>();
	//스테이트 종료
	private Subject<Unit> end = new Subject<Unit>();

	public IObservable<Unit> OnBeginStream => begin.Share();
	public IObservable<Unit> OnUpdateStream => StateStream(this.UpdateAsObservable());
	public IObservable<Unit> OnEndStream => end.Share();

	protected IObservable<T> StateStream<T>(IObservable<T> source)
	{
		return source
			//begin스트림에서 OnNext가 실행되면서
			.SkipUntil(OnBeginStream)
			//End스트림에서OnNext가 실행되기전까지
			.TakeUntil(OnEndStream)
			.RepeatUntilDestroy(gameObject)
			.Share(); //옵저버 추가시 자동으로 connect
	}
	public void RegisterMachine(StateMachine machine)
	{
		this.machine = machine;
	}
	
	public void BeginState()
	{
		begin.OnNext(default);
	}

	public void EndState()
	{
		end.OnNext(default);
	}
	protected void ChangeState<T>() where T : State
	{
		machine.ChangeState<T>();
	}
}