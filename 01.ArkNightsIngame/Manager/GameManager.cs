using System;
using Bam.Singleton;
using TMPro;
using UnityEngine;


public class GameManager : Singleton<GameManager>
{
	[SerializeField] private TileManager tileManager;
	[SerializeField] private OperatorManager operatorManager;

	private Stage stage;
	private StateMachine stateMachine;

	public OperatorManager OperatorManager => operatorManager;
	public TileManager TileManager => tileManager;
	public Stage Stage => stage;

	protected override void Init()
	{
		stage = FindFirstObjectByType<Stage>();

		float time = 0;
		stateMachine = new StateMachine();

		stateMachine.AddState((int)GameState.Init, new State().OnEnter(() =>
		{
			stage.Init();
			
		}).AddCondition((int)GameState.Playing, () => true));

		stateMachine.AddState((int)GameState.Playing, new State().OnEnter(() =>
			{

			}).OnStay(() =>
			{
				time += Time.deltaTime;
				stage.UpdateStage(CustomTime.deltaTime);
			})
			.AddCondition((int)GameState.Lose, () => stage.Life <= 0)
			.AddCondition((int)GameState.Win, () => false));
		
		stateMachine.AddState((int)GameState.Lose, new State().OnEnter(() =>
		{

		}));
	}

	public void DeployOperator(Operator op)
	{
		stage.Cost.Value -= op.Cost.Value;
		stage.CharacterLimit.Value--;
		operatorManager.AddOperator(op);
	}
	public void RemoveOperator(Operator op)
	{
		stage.CharacterLimit.Value++;
		operatorManager.RemoveOperator(op);
	}

	private void Start()
	{
		stateMachine.ChangeState((int)GameState.Init);
	}

	private void Update()
	{
		stateMachine.Update();
	}
}