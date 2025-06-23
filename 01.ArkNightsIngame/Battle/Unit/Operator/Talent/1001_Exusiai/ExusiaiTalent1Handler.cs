using System.Collections.Generic;
using System.Linq;

public class ExusiaiTalent1Handler : TalentHandler
{
	private Operator owner;
	private float speedBuff;

	public override void Initialize(Operator op)
	{
		owner = op;
		speedBuff = data.Effects.FirstOrDefault(x => x.type is AttributeType.AtkSpeed).value;
		owner.SetAdditionalAttribute(new AttributeModifier(AttributeType.AtkSpeed, speedBuff, AttributeModifierType.Add));
	}
}