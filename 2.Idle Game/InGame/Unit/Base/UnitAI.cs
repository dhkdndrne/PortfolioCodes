using System;
using UnityEngine;
using Bam.Extensions;
using UniRx;

public enum UnitAffiliation
{
    Enemy,
    Player
}

public class UnitAI : MonoBehaviour
{
    [field: SerializeField]
    public UnitAffiliation UnitAffiliation { get; private set; }
    
    private IDisposable updateSubscription;
    
    public UnitHp Target { get; private set; }
    public UnitBase unitBase { get; private set; }
    public Animator Animtor { get; private set; }
    public UnitHp MyHp { get; private set; }
    
    private void OnEnable()
    {
        updateSubscription = Observable.EveryUpdate()
            .Subscribe(_ => UpdateCoolTime(Time.deltaTime))
            .AddTo(this);
    }
    private void OnDisable()
    {
        updateSubscription?.Dispose();
    }
    
    private void Awake()
    {
        unitBase = GetComponent<UnitBase>();
        Animtor = transform.GetComponent<Animator>();
        MyHp = GetComponent<UnitHp>();
    }
    
    public void Init()
    {
        unitBase.Init(this);
        GetComponent<StateMachine>().Init();
    }
    
    public void FindEnemy()
    {
        if (UnitAffiliation == UnitAffiliation.Enemy)
        {
            Target = Player.Instance.PlayerUnit.MyHp;
        }
        else
        {
            if (StageManager.Instance.SpawnedEnemyList.Count == 0)
            {
                Target = null;
                return;
            }
            
            UnitHp target = null;
            float minDistance = int.MaxValue;
            
            foreach (var enemy in StageManager.Instance.SpawnedEnemyList)
            {
                float distance = Vector2.SqrMagnitude(transform.position - enemy.transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    target = enemy;
                }
            }

            Target = target;
        }
    }
    
    /// <summary>
    /// 타겟이 공격범위 내에 들어왔는지 판단
    /// </summary>
    /// <returns></returns>
    public bool CheckTargetInAttackRange()
    {
        if (Target is null)
            return false;

        return GetTargetSqrDistance() <= Extensions.Pow(unitBase.Stat.AtkRange,2);
    }

    public float GetTargetSqrDistance()
    { 
        if (Target is null)
            return float.MaxValue;
        
        return Vector2.SqrMagnitude(transform.position - Target.transform.position);
    }
    
    public void UpdateCoolTime(float deltaTime)
    {
        unitBase.UpdateCoolTime(deltaTime);
    }

}
