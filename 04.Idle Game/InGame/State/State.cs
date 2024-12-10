using System;
using UnityEngine;

[RequireComponent(typeof(StateMachine))]
public abstract class State : MonoBehaviour
{
	public StateMachine StateMachine { get; private set; }

	protected Animator animator;
	protected UnitAI myAI;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	public void Init(StateMachine stateMachine,UnitAI unitAI)
	{
		myAI = unitAI;
		StateMachine = stateMachine;
	}
	
	public abstract void BeginState();   
	public abstract void UpdateState();       
	public abstract void EndState();   
	
	protected void ChangeState<T>() where T : State
	{
		StateMachine.ChangeState<T>();
	}
}