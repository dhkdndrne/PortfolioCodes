using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private Dictionary<Type, State> stateDic = new();
    private State curState;

    private void Awake()
    {
        foreach (var state in GetComponents<State>())
        {
            Register(state);
        }
    }
    
    public void StartFsm()
    {
        curState = stateDic[typeof(State_Input)];
        curState.BeginState();
    }
    
    /// <summary>
    /// 스테이트 등록
    /// </summary>
    /// <param name="state"></param>
    public void Register(State state)
    {
        stateDic.Add(state.GetType(), state);
        state.RegisterMachine(this);
    }
    public void ChangeState<T>() where T : State
    { 
        //현재 스테이트 종료
        curState.EndState();

        //다음 스테이트 개시
        curState = stateDic[typeof(T)];
        curState.BeginState();
    }
}
