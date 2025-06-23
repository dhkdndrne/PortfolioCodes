using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OperatorObjectPool
{
	[SerializeField] private List<Operator> operatorList;
	private Dictionary<OperatorID, Operator> operatorDic = new Dictionary<OperatorID, Operator>();

	public event Action<int,float> OnOperatorRetreat; // index cost redeployTime
	
	// 퇴각한 오퍼레이터 인덱스 반환.
	// 오퍼레이터 배치
	public void Init()
	{
		for (int i = 0; i < operatorList.Count; i++)
		{
			var obj = GameObject.Instantiate(operatorList[i]);
			var controller = obj.GetComponent<OperatorController>();

			int index = i;
			controller.Op.OnDeath += () =>
			{
				float redeployTime = controller.Op.Attribute.GetRedeployTime();
				redeployTime = Bam.Extensions.Extensions.DecreasePercent(redeployTime, controller.Op.Attribute.GetAddTotalExtraAttribute(AttributeType.ReDeployTime));
				OnOperatorRetreat?.Invoke(index,redeployTime);
			};
			
			controller.Init();
			controller.gameObject.SetActive(false);
			operatorDic.Add(operatorList[i].OperatorID, controller.Op);

			// 스쿼드 배치시 적용될 재능 있으면 적용
			foreach (var talent in controller.Op.Talents)
			{
				if (talent is ISquadDeployHandler squadDeployHandler)
				{
					squadDeployHandler.HandleSquadDeploy();
				}
			}
		}
	}
	public Operator GetOperator(OperatorID id)
	{
		var op = operatorDic[id];
		return op;
	}

	public IEnumerable<Operator> GetOperators()
	{
		foreach (var op in operatorDic)
		{
			yield return op.Value;
		}
	}
}