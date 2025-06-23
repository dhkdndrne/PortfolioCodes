using Cysharp.Threading.Tasks;
using UnityEngine;

public class MyrtleTalent1Handler : TalentHandler
{
	private bool isDeployed;
	private float heal;
	public override void Initialize(Operator op)
	{
		op.onDeploy += HealVanguard().Forget;
		op.OnDeath += () => isDeployed = false;
		
		heal = data.Effects[0].type == AttributeType.Healing ? data.Effects[0].value : 0;
	}

	private async UniTaskVoid HealVanguard()
	{
		isDeployed = true;
		var operatorManager = GameManager.Instance.OperatorManager;
		while (isDeployed)
		{
			foreach (var op in operatorManager.GetDeployedOperators())
			{
				if(op.IsDead()) continue;
				if(op.OperatorClass is not Operator_Class.Vanguard) continue;
				op.Heal(heal);
			}
			
			await UniTask.Delay(1000);
		}
	}
}
