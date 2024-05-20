using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Bone : MonoBehaviour
{
    private PoolObject poolObject;
    private IDisposable updateSubscription;
    
    private void Awake()
    {
        poolObject = GetComponent<PoolObject>();
    }

    private void OnEnable()
    {
        updateSubscription = this.UpdateAsObservable().
            Delay(TimeSpan.FromSeconds(3)).
            Subscribe(_ =>
        {
            ObjectPoolManager.Instance.Despawn(poolObject);
        }).AddTo(this);
    }

    private void OnDisable()
    {
        updateSubscription?.Dispose();
    }
}
