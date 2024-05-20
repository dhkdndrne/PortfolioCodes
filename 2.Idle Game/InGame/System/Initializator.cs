using System.Collections;
using System.Collections.Generic;
using Bam.Singleton;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

public class Initializator : Singleton<Initializator>
{
    public Action onFirstInit; 
    public Action onSecondInit; 
    public Action onAfterPlayerInit; 
    
    private void Start()
    {
        Application.targetFrameRate = 60;
        Init().Forget();
    }

    private async UniTaskVoid Init()
    {
        onFirstInit?.Invoke();
        await UniTask.Yield();
        
        onSecondInit?.Invoke();
        await UniTask.Yield();
        
        Player.Instance.PlayerUnit.Init();
        await UniTask.Yield();
        
        onAfterPlayerInit?.Invoke();
        await UniTask.Yield();
      
    }
}
