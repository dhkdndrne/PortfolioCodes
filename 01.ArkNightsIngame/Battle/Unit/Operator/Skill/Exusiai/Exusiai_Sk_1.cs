using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Exusiai_Sk_1 : Skill
{
    private float damageDelta;
    protected override void PostInit(Operator op,int index)
    {
        var data = DataManager.Instance.OperatorData.GetOperatorSkillData(id);
        damageDelta = data.GetSkillLevelData(10).Attributes[0].Value;
    }
    
    public override void SetSkill()
    {
        isEnd = false;
        isActivating = false;
        base.SetSkill();
    }
    
    public override void Use(Operator caster, HashSet<Unit> targetList)
    {
        float damage = caster.Attribute.AttackPower * damageDelta;

        var target = targetList.FirstOrDefault();
        target?.Hit(caster);

        isEnd = true;
    }
    public override bool IsSkillEnd()
    {
        if(isEnd)
        {
            isActivating = false;
            Sp.Value = 0;
        }
        return isEnd;
    }
    public override bool UpdateSkill(HashSet<Unit> targetList)
    {
        if (targetList.Count > 0 && !isActivating)
        {
            isActivating = true;
            return true;
        }
        return false;
    }
}
