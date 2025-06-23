using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using DG.Tweening;

public class State_Start : State
{
	[SerializeField] private TargetPopUp targetPopUp;
	private void Awake()
	{
		OnBeginStream.Subscribe(async _ =>
		{
			await targetPopUp.MovePopUp();

			var board = GameManager.Instance.Board;
			await board.PlayBoardAnim();

			ChangeState<State_Input>();

		}).AddTo(this);

	}
}