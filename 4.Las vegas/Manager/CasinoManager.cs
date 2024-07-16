using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Bam.Singleton;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
public class CasinoManager : Singleton<CasinoManager>
{
	private BankSystem bankSystem;

	[field: SerializeField] public Casino[] Casinos { get; private set; } = new Casino[6];

	public PhotonView PV { get; private set; }

	private void Start()
	{
		PV = GetComponent<PhotonView>();
		bankSystem = GetComponent<BankSystem>();

		GameManager.Instance.InitAction += bankSystem.Init;
		//GameManager.Instance.InitAction += () => PV.RPC(nameof(RPC_InitCasino), RpcTarget.All);
	}

	[PunRPC]
	public void RPC_InitCasino()
	{
		foreach (var casino in Casinos)
		{
			casino.SetPrize(bankSystem.GetPrizeList());
		}
	}

	[PunRPC]
	public void RPC_CalculateCasinoPrize()
	{
		foreach (var casino in Casinos)
		{
			if (casino.SortedList != null && casino.SortedList.Count > 0)
			{
				casino.PV.RPC(nameof(casino.RPC_DeleteEqualValuePlayer), RpcTarget.MasterClient);
				casino.PV.RPC(nameof(casino.RPC_GivePrize), RpcTarget.MasterClient);
			}
			casino.PV.RPC(nameof(casino.RPC_DeleteMoneyCard),RpcTarget.All);
		}
		
		GameManager.Instance.StartNextRound().Forget();
	}

	/// <summary>
	/// 남은 중립 주사위 랜덤한 카지노에 배팅
	/// </summary>
	[PunRPC]
	public void RPC_BetRemainSpecialDice(int diceAmount)
	{
		while (diceAmount > 0)
		{
			var casino = Casinos[Random.Range(0, Casinos.Length)];
			var randDiceAmount = Random.Range(0, diceAmount + 1);

			if (casino.PV.IsMine)
				casino.PV.RPC(nameof(casino.RPC_BetDice), RpcTarget.All, string.Empty, 0, randDiceAmount);
			diceAmount -= randDiceAmount;
		}
	}
}