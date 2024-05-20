using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitAI))]
public abstract class UnitBase : MonoBehaviour
{
    protected UnitAI unitAI;
    protected bool isAttack;
    
    public UnitStat Stat { get; protected set; } // 유닛 스탯
    
    public abstract void Init(UnitAI unitAI);

    public virtual void Attack()
    {
        isAttack = true;
        unitAI.Animtor.SetTrigger(AnimationHash.Attack_ANIM_HASH);
    }
    
    protected abstract void OnAttack();
    protected abstract void EndAttack();
    
    public bool CanAttack()
    {
        return !isAttack && Stat.AtkCoolTime <= 0;
    }

    protected virtual void SetCoolTime()
    {
        Stat.AtkCoolTime = 1 / Stat.AtkSpeed;
    }

    public virtual void UpdateCoolTime(float deltaTime)
    {
        if (Stat.AtkCoolTime <= 0)
            return;
        
        Stat.AtkCoolTime -= deltaTime;

        if (Stat.AtkCoolTime <= 0)
            Stat.AtkCoolTime = 0;
    }

    protected abstract void Dead();

}
