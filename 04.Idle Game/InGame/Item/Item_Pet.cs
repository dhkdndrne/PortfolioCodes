using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Pet : ItemBase
{
    private double damage;
    private float coolTime;
    private float time;
    public RuntimeAnimatorController AC { get; private set; }
    
    public bool CanAttack { get; private set; }
    public bool IsEquipped { get; private set; }
    
    public void Init(int id,bool isLock, string itemName,ItemRankType rank, int level, int maxLevel, int piece, int maxPiece, OwnEffect ownEffect,string description,double damage,float coolTime)
    {
        base.Init(id,isLock, itemName,rank, level, maxLevel, piece, maxPiece, ownEffect,description);

        this.damage = damage;
        this.coolTime = coolTime;

        AC = Resources.Load<RuntimeAnimatorController>($"AC/Pet_{id}");
    }
    
    public void Equip()
    {
        IsEquipped = true;
    }

    public void UnEquip()
    {
        IsEquipped = false;
    }
    private void Attack(UnitHp target)
    {
        if (target == null)
            return;
        
        target.Hit(damage);
        CanAttack = false;
        time = coolTime;
    }

    public void UpdateCoolTime(float deltaTime)
    {
        if (CanAttack)
        {
            var target = GetTarget();
            
            if (target == null)
                return;
            
            Attack(target);
            return;
        }

        time -= deltaTime;

        if (time <= 0)
        {
            CanAttack = true;
            time = 0;
        }
    }

    private UnitHp GetTarget()
    {
        var list = StageManager.Instance.SpawnedEnemyList;

        if (list.Count == 0)
            return null;
        
        int randIndex = Random.Range(0, list.Count);

        while (randIndex > list.Count)
        {
            randIndex = Random.Range(0, list.Count);
        }
        return StageManager.Instance.SpawnedEnemyList[randIndex];
    }

    public override string GetDescription()
    {
        return $"<color=red>{coolTime}초</color> 마다 <color=green>{NumberTranslater.TranslateNumber(damage)}</color> 데미지로 공격";
    }
}
