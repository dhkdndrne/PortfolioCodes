using System.Collections.Generic;
using System;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    #region Inspector

    [SerializeField] private State initState;

    #endregion
    
    #region Field

    private State currentState;
    private Dictionary<Type, State> stateDic = new Dictionary<Type, State>();

    #endregion
    
    #region Property
    
    public State CurrentState => currentState;
    
    #endregion

    private void Awake()
    {
        foreach (var state in GetComponents<State>())
        {
            RegistState(state);
        }
    }

    public void Init()
    {
        if (initState == null)
            return;

        currentState = initState;
        currentState.BeginState();
    }

    public void RegistState(State state)
    {
        stateDic.Add(state.GetType(),state);
        state.Init(this,GetComponent<UnitAI>());
    }

    public void ChangeState<T>() where T : State
    {
        currentState.EndState();
        currentState = stateDic[typeof(T)];
        currentState.BeginState();
    }

    private void Update()
    {
        currentState?.UpdateState();
    }
}
