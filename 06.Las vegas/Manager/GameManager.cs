using System;
using System.Collections;
using System.Collections.Generic;
using Bam.Singleton;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UniRx;
using UnityEngine;

public class GameManager : Singleton<GameManager>, IPunObservable
{
	#region Inspector

	public TurnSystem TurnSystem { get; private set; }
	[field: SerializeField] public IngamePresenter IngamePresenter { get; private set; }

    #endregion

    #region Field

	private PhotonView pv;
	public Action InitAction;
	public Action GameOverAction;

	public GameManagerModel Model { get; private set; } = new();

	#endregion
	
	protected override void Awake()
	{
		base.Awake();
		pv = GetComponent<PhotonView>();
		TurnSystem = GetComponent<TurnSystem>();

		PhotonManager.Instance.LeftRoomSubject.Subscribe(index =>
		{
			if (Model.isGameStarted)
			{
				TurnSystem.PlayerList.RemoveAt(index);
				
				if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
					pv.RPC(nameof(RPC_GameOver), RpcTarget.All);
			}
		}).AddTo(gameObject);
	}

	/// <summary>
	/// Master Client만 실행
	/// </summary>
	public async UniTaskVoid InitGame()
	{
		InitAction?.Invoke();
		CasinoManager.Instance.PV.RPC(nameof(CasinoManager.Instance.RPC_InitCasino), RpcTarget.All);

		TurnSystem.SetRandomTurn();

		pv.RPC(nameof(RPC_SetGameState), RpcTarget.All, true);

		await UniTask.Delay(3000);
		pv.RPC(nameof(TurnSystem.RPC_StartNextTurn), RpcTarget.MasterClient);
		pv.RPC(nameof(RPC_IncreaseRoundAndShowAnim), RpcTarget.All);
	}
	
	[PunRPC]
	private void RPC_SetGameState(bool value) => Model.isGameStarted = value;

	[PunRPC]
	private void RPC_IncreaseRoundAndShowAnim()
	{
		Model.Round.Value++;
		IngamePresenter.ShowRoundAnimation().Forget();
	}

	public async UniTaskVoid StartNextRound()
	{
		if (Model.Round.Value >= 1)
		{
			UtilClass.DebugLog("게임 끝났땅");
			pv.RPC(nameof(RPC_GameOver), RpcTarget.All);
		}
		else
		{
			pv.RPC(nameof(RPC_IncreaseRoundAndShowAnim), RpcTarget.All);
			CasinoManager.Instance.PV.RPC(nameof(CasinoManager.Instance.RPC_InitCasino), RpcTarget.All);

			await UniTask.Delay(3000);
			pv.RPC(nameof(TurnSystem.RPC_SetNextRoundTurn), RpcTarget.MasterClient);
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(Model.Round.Value);
		}
		else
		{
			Model.Round.Value = (int)stream.ReceiveNext();
		}
	}

	[PunRPC]
	private void RPC_GameOver()
	{
		TurnSystem.PlayerList.Sort((p1, p2) => p1.Model.Money.Value < p2.Model.Money.Value ? 1 : -1);
		GameOverAction?.Invoke();
		Model.isGameStarted = false;
		Model.Round.Value = 0;
		
		TurnSystem.ClearPlayerList();
		PhotonNetwork.CurrentRoom.IsOpen = true;
	}
}
public class GameManagerModel
{
	public ReactiveProperty<int> Round { get; private set; } = new();
	public bool isGameStarted;

}