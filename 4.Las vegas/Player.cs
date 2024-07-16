using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Player : MonoBehaviourPun
{
	private PhotonView pv;
	private DiceManager diceManager;

	public PhotonView PV => pv;
	public PlayerModel Model { get; private set; } = new();

	private void Awake()
	{
		pv = GetComponent<PhotonView>();
		diceManager = FindObjectOfType<DiceManager>();

		GameManager.Instance.TurnSystem.PlayerList.Add(this);
		GameManager.Instance.IngamePresenter.DiceRollAction += RollDice;

		this.UpdateAsObservable().Where(_ => pv.IsMine && Model.IsMyTurn.Value && Model.IsBettingTime.Value).Subscribe(_ =>
		{
			if (Input.GetMouseButtonDown(0))
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit[] hits = Physics.RaycastAll(ray);

				foreach (var hit in hits)
				{
					if (hit.transform.TryGetComponent(out Casino casino))
					{
						int casinoNum = casino.CasinoNum;
						//주사위가 없으면 리턴
						if (diceManager.DiceNumberDic[casinoNum] == 0 && diceManager.DiceNumberDic[-casinoNum] == 0) return;

						//카지노에 주사위 베팅
						casino.PV.RPC(nameof(casino.RPC_BetDice), RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, diceManager.DiceNumberDic[casinoNum], diceManager.DiceNumberDic[-casinoNum]);

						//주사위 개수 업데이트
						pv.RPC(nameof(RPC_UpdateDiceAmount), RpcTarget.All, diceManager.DiceNumberDic[casinoNum], diceManager.DiceNumberDic[-casinoNum]);

						//DiceManager 딕셔너리 초기화
						diceManager.ResetDictionary();

						//베팅 끝
						pv.RPC(nameof(RPC_BettingTime), RpcTarget.All, false);
						pv.RPC(nameof(RPC_SetMyTurn), RpcTarget.All, false);

						//다음 턴 요청
						GameManager.Instance.TurnSystem.PV.RPC(nameof(GameManager.Instance.TurnSystem.RPC_StartNextTurn), RpcTarget.MasterClient);
					}
				}
			}
		}).AddTo(gameObject);
		
	}


	public void InitPlayer(int playerNumber)
	{
		Model.InitModel(playerNumber,diceManager.DICE_COUNT, diceManager.SpecialDiceCount);
		GameManager.Instance.IngamePresenter.InitUIEvent(this);
		
		if (pv.IsMine && PhotonManager.Instance.IsMaster() && diceManager.RemainSpecialDice > 0)
			CasinoManager.Instance.PV.RPC(nameof(CasinoManager.Instance.RPC_BetRemainSpecialDice), RpcTarget.All, diceManager.RemainSpecialDice);
		
	}

	private void RollDice()
	{
		if (pv.IsMine && Model.IsMyTurn.Value)
		{
			pv.RPC(nameof(RPC_RollDice), RpcTarget.MasterClient);
		}
	}

	[PunRPC]
	private void RPC_UpdateDiceAmount(int diceAmount, int sDiceAmount)
	{
		Model.UpdateDiceAmount(diceAmount, sDiceAmount);
	}

	[PunRPC]
	public void RPC_SetMyTurn(bool isMyTurn)
	{
		if (pv.IsMine)
			Model.IsMyTurn.Value = isMyTurn;
	}

	/// <summary>
	/// 마스터에게 주사위 굴리는거 요청
	/// </summary>
	[PunRPC]
	private void RPC_RollDice() => diceManager.RollDice().Forget();

	[PunRPC]
	public void RPC_BettingTime(bool isBettingTime) => Model.IsBettingTime.Value = isBettingTime;

	[PunRPC]
	public void RPC_GetMoney(int money) => Model.Money.Value += money;
}