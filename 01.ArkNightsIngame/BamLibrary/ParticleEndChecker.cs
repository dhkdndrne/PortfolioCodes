using UnityEngine;

public class ParticleEndChecker : MonoBehaviour
{
    private void Awake()
    {
        var main = GetComponent<ParticleSystem>().main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    /// <summary>
    /// 파티클이 끝나면 호출
    /// </summary>
    private void OnParticleSystemStopped()
    {
        ObjectPoolManager.Instance.DeSpawn(gameObject);
    }
}
