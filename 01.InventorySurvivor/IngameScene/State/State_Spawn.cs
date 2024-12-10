using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class State_Spawn : State
{
    [SerializeField] private GameObject spawnFX;
    
    public override void BeginState()
    {
        WaitSpawn().Forget();
    }
    
    public override void UpdateState()
    {

    }
    public override void EndState()
    {
       CreatureManager.Instance.EnemyList.Add(myAI.UnitBase);
    }

    private async UniTaskVoid WaitSpawn()
    {
        spawnFX.SetActive(true);
        myAI.SkinnedRenderer.enabled = false;
        
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        myAI.SkinnedRenderer.enabled = true;
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        
        spawnFX.SetActive(false);
        ChangeState<State_Idle>();
    }
}
