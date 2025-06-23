using System;
using UnityEngine;

public class EnemyEventHandler : MonoBehaviour
{
	public event Action OnAttackAction;
	public event Action OnEndAction;
	
	private void OnAttack()
	{
		OnAttackAction?.Invoke();
	}

	private void End()
	{
		OnEndAction?.Invoke();
	}
}
