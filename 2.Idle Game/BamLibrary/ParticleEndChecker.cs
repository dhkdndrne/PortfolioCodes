using UnityEngine;

public class ParticleEndChecker : MonoBehaviour
{
    private PoolObject poolObject;

    private void Awake()
    {
        poolObject = GetComponentInParent<PoolObject>();

        var main = GetComponent<ParticleSystem>().main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    /// <summary>
    /// 파티클이 끝나면 호출
    /// </summary>
    private void OnParticleSystemStopped()
    {
        ObjectPoolManager.Instance.Despawn(poolObject);
    }
}
