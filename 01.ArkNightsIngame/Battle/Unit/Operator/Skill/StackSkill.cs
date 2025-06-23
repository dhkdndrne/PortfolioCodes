using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class StackSkill : Skill
{
    public ObservableValue<int> Stack { get; protected set; } = new ObservableValue<int>(0);
    protected int maxStack;
    public int MaxStack => maxStack;
    
    // StackSkill에서는 추가로 스택 관련 초기화 작업을 수행
    protected override void PostInit(Operator op, int index)
    {
        // 여기서 스택 값만 초기화
        Stack.Value = 0;
        
        // id와 index는 이미 base.Init()에서 설정됨.
        var data = DataManager.Instance.OperatorData.GetOperatorSkillData(id);
        var skillData = data.GetSkillLevelData(10);
        maxStack = skillData.StackCount;
    }
    
    public override void Use(Operator caster, HashSet<Unit> targetList)
    {
        
    }
    
    public override bool IsSkillEnd()
    {
        return false;
    }
    
    public override bool UpdateSkill(HashSet<Unit> targetList)
    {
        return false;
    }
}
