using System.Linq;
using UnityEngine;

public class SilverAshTalent1Handler : TalentHandler, ISquadDeployHandler
{
	public override void Initialize(Operator op)
	{
		float damage = data.Effects.FirstOrDefault(x => x.type == AttributeType.Damage).value;
		op.SetAdditionalAttribute(new AttributeModifier(AttributeType.Damage, damage, AttributeModifierType.Add));

	}
	public void HandleSquadDeploy()
	{
		float redeploy = data.Effects.FirstOrDefault(x => x.type == AttributeType.ReDeployTime).value;
		foreach (var oper in GameManager.Instance.OperatorManager.GetAllOperators())
		{
			oper.SetAdditionalAttribute(new AttributeModifier(AttributeType.ReDeployTime, redeploy, AttributeModifierType.Add));
		}
	}
}