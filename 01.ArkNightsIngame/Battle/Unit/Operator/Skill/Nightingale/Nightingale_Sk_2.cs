using System.Collections.Generic;

public class Nightingale_Sk_2 : StackSkill
{
    public override void SetSkill()
    {
        isEnd = false;
        isActivating = false;
        base.SetSkill();
    }
    public override bool CanUse()
    {
        bool condition_1 = Stack.Value > 0 && op.Controller.AtkCoolTime <= 0;
        bool condition_2 = false;
        
        foreach (Operator target in op.Controller.TargetList)
        {
            condition_2 |= !target.IsDead() && target.Attribute.HpRatio.Value < 1;
        }
        
        return condition_1 && condition_2;
    }

    public override void ChargeSP(float value)
    {
        base.ChargeSP(value);
        
        if (Sp.Value >= requireSp)
        {
            if(Stack.Value >= maxStack)
                Stack.Value = maxStack;
            else
            {
                Stack.Value++;
                Sp.Value = 0;
            }
        }
    }

    public override void Use(Operator caster, HashSet<Unit> targetList)
    {
        bool used = false;
        
        foreach (Unit target in targetList)
        {
            if(target.IsDead()) continue;

            target.Heal(caster.GetFinalDamage());
            
            // BuffGroup을 생성하여 대상에 버프들을 묶어서 관리
            BuffGroup buffGroup = new BuffGroup(target);

            // 보호막 버프 적용
            float shieldAmount = caster.GetFinalDamage() * deltaValueDic[AttributeType.Damage];
            Buff_Shield shieldBuff = new Buff_Shield(duration,true,shieldAmount);
            buffGroup.AddBuff(shieldBuff);

            // 마법 저항 증가 버프 적용
            Buff_MagicResist magicResistBuff = new Buff_MagicResist(duration,true,deltaValueDic[AttributeType.MagicResistance]);
            buffGroup.AddBuff(magicResistBuff);

            // 대상에게 버프 그룹을 적용
            buffGroup.Activate(duration);
            used = true;
        }

        if (used)
        {
            Stack.Value--;
            isEnd = true;
            Debug.LogTry($"스킬 사용 {Stack.Value}");
        }
    }
    
    public override bool IsSkillEnd()
    {
        return isEnd;
    }
    
    public override bool UpdateSkill(HashSet<Unit> targetList)
    {
        if (CanUse() && !isActivating)
        {
            isActivating = true;
            return true;
        }
        
        return false;
    }

    public override void EndSkillAnim()
    {
        op.Controller.SetAtkCoolTime();
        isActivating = false;
    }
}
