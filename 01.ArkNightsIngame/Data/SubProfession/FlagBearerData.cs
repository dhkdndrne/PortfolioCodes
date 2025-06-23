using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/SubProfession/FlagBearerData")]
public class FlagBearerData : SubProfessionData
{
	public override void ApplyTrait(OperatorController op)
	{
		WaitUntilSkillEnd(op.Op).Forget();
	}

	private async UniTaskVoid WaitUntilSkillEnd(Operator op)
	{
		int block = op.MaxBlock;
		op.Block = 0;
		await UniTask.WaitUntil(() => op.GetSkill().IsSkillEnd());
		op.Block = block;
	}
}
