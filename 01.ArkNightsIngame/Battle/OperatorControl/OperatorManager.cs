using System;
using System.Collections.Generic;
using UnityEngine;


public class OperatorManager : MonoBehaviour
{
	[SerializeField] private OperatorObjectPool operatorObjectPool;
	private OperatorSlotController operatorSlotController;

	public event Action<Operator> OnOperatorDeployed;
	public event Action<Operator> OnOperatorRemoved;

	[SerializeField]
	private List<Operator> deployedOperators = new List<Operator>();
	
	//외부에서 수정 방지
	public IReadOnlyList<Operator> GetDeployedOperators() => deployedOperators.AsReadOnly();
	
	private void Awake()
	{
		operatorSlotController = FindFirstObjectByType<OperatorSlotController>();
	
		operatorObjectPool.Init();
		operatorSlotController.Init(this);
		
		operatorObjectPool.OnOperatorRetreat += (index, respawnTime) =>
		{
			operatorSlotController.StartOperatorRespawn(index, respawnTime);
		};
	}

	public void AddOperator(Operator op)
	{
		if (deployedOperators.Contains(op))
			return;

		deployedOperators.Add(op);
		OnOperatorDeployed?.Invoke(op);
	}

	public void RemoveOperator(Operator op)
	{
		if (!deployedOperators.Contains(op))
			return;
		
		deployedOperators.Remove(op);
		OnOperatorRemoved?.Invoke(op);
	}

	public Operator GetOperator(OperatorID id) => operatorObjectPool.GetOperator(id);
	public IEnumerable<Operator> GetAllOperators()
	{
		return operatorObjectPool.GetOperators();
	}
}