using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class State_StageClear : State
{
    private void Start()
    {
        OnBeginStream.Subscribe(_ =>
        {
            ChangeState<State_Input>();
        }).AddTo(this);
    }
}
