using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class StateMachine
{
	private Dictionary<int, State> dic = new();
	private Dictionary<int, Func<bool>> conditions = new Dictionary<int, Func<bool>>();
	[SerializeField] private State curState;

	public void ChangeState(int state, bool withoutEnd = false)
	{
		if (state == curState?.stateType || !dic.TryGetValue(state, out State next)) return;

		if (!withoutEnd)
			curState?.end.Invoke();
		
		next.enter.Invoke();
		curState = next;
	}

	public void AddState(int stateType, State state)
	{
		dic.Add(stateType, state);
		state.stateType = stateType;
	}

	public void Update()
	{
		if (curState == null) return;
		curState.stay.Invoke();

		// 전역 조건 검사: 현재 상태와 다른 상태에 대해 조건이 true이면 전환
		foreach (KeyValuePair<int, Func<bool>> entry in conditions.Where(entry => entry.Key != curState.stateType && entry.Value()))
		{
			// dic에서 해당 enum에 해당하는 상태를 가져와 전환
			ChangeState(entry.Key);
			return;
		}

		// 하위 조건 검사: 현재 상태 내부 조건이 true이면 전환
		foreach (KeyValuePair<int, Func<bool>> entry in curState.conditions.Where(entry => entry.Value()))
		{
			// dic에서 해당 enum에 해당하는 상태를 가져와 전환
			ChangeState(entry.Key);
			return;
		}
	}
	public void AddCondition(int status, Func<bool> condition)
	{
		conditions.Add(status, condition);
	}

	public void Clear()
	{
		dic.Clear();
		conditions.Clear();
	}
}

[Serializable]
public class State
{
	private static Action EmptyAction = () => { };
	public int stateType;

	public Action enter = EmptyAction;
	public Action stay = EmptyAction;
	public Action end = EmptyAction;
	public Dictionary<int, Func<bool>> conditions = new Dictionary<int, Func<bool>>();

	public State OnEnter(Action action)
	{
		enter = action;
		return this;
	}

	public State OnStay(Action action)
	{
		stay = action;
		return this;
	}

	public State OnEnd(Action action)
	{
		end = action;
		return this;
	}

	public State AddCondition(int status, Func<bool> condition)
	{
		conditions.Add(status, condition);
		return this;
	}
}