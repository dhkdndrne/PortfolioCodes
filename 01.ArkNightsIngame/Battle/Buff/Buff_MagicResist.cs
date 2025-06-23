using UnityEngine;

public class Buff_MagicResist : Buff
{
    public Buff_MagicResist(float duration, bool isGroup,float bonus) : 
        base(duration, isGroup)
    {
        Bonus = bonus;
    }
    public float Bonus { get; private set; }
    private AttributeModifier attributeModifier;
    
    public override void Apply(Unit target)
    {
        attributeModifier = new AttributeModifier(AttributeType.MagicResistance, Bonus, AttributeModifierType.Add);
        // 대상의 마법 저항력에 Bonus를 추가
        target.SetAdditionalAttribute(attributeModifier);
    }

    public override void Remove(Unit target)
    {
        // 버프 제거 시 대상의 마법 저항력에서 Bonus를 제거
        target.RemoveAdditionalAttribute(attributeModifier);
    }
}
