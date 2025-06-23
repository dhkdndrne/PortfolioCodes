using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
	private PhotonView pv;

	private List<Dice> diceList = new();
	private List<UniTask<(int, Define.DiceType)>> rollResultList = new();
	public Dictionary<int, int> DiceNumberDic { get; private set; } = new() // 주사위 눈금 수 저장 (음수는 중립 주사위)
	{
		{ -1, 0 }, { -2, 0 }, { -3, 0 }, { -4, 0 }, { -5, 0 }, { -6, 0 },
		{ 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }
	};

	public readonly int DICE_COUNT = 8;
	private readonly string DICE_PREFAB_NAME = "Dice";

	public int RemainSpecialDice { get; private set; }
	public int SpecialDiceCount { get; private set; }

	private void Start()
	{
		pv = GetComponent<PhotonView>();
		GameManager.Instance.InitAction += InitDice;
	}

	private void InitDice()
	{
		//	두명이서 플레이하면 중립주사위 4개
		//	3~4명이서 게임하면 중립 주사위 2개씩
		//	3명이서 플레이하면 중립주사위 2개는 눈금에 맞는 카지노에 넣음

		int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
		SpecialDiceCount = playerCount == 2 ? 4 : 2;
		RemainSpecialDice = playerCount == 3 ? 2 : 0;

		bool hasDice = diceList.Count > 0;

		
		for (int i = 0; i < DICE_COUNT + SpecialDiceCount; i++)
		{
			if (!hasDice)
				diceList.Add(PhotonNetwork.Instantiate(DICE_PREFAB_NAME, new Vector3(0, 5, 0), Quaternion.identity).GetComponent<Dice>());

			if (DICE_COUNT + SpecialDiceCount - i <= SpecialDiceCount)
				diceList[i].ChangeDiceColor(Define.DiceType.Special);
		}

		pv.RPC(nameof(RPC_RefreshDiceAmount), RpcTarget.Others, SpecialDiceCount, RemainSpecialDice);

		foreach (var dice in diceList)
		{
			dice.SetActivate(false);
		}
	}

	public async UniTaskVoid RollDice()
	{
		Player player = GameManager.Instance.TurnSystem.NowPlayingPlayer;
		rollResultList.Clear();

		for (int i = 0; i < player.Model.Dice.Value; i++)
		{
			diceList[i].SetActivate(true);
			rollResultList.Add(diceList[i].Roll());
		}

		for (int i = player.Model.SpecialDice.Value; i > 0; i--)
		{
			diceList[7 + i].SetActivate(true);
			rollResultList.Add(diceList[7 + i].Roll());
		}

		var diceResultList = await UniTask.WhenAll(rollResultList);

		foreach (var value in diceResultList)
		{
			int dot = value.Item2 == Define.DiceType.Special ? value.Item1 * -1 : value.Item1;
			DiceNumberDic[dot]++;
		}

		for (int i = 0; i < 6; i++)
		{
			GameManager.Instance.IngamePresenter.ShowDiceUI(i, DiceNumberDic[i + 1], DiceNumberDic[-(i + 1)]);
			pv.RPC(nameof(RPC_RefreshRolledDice), RpcTarget.All, i + 1, DiceNumberDic[i + 1]);
			pv.RPC(nameof(RPC_RefreshRolledDice), RpcTarget.All, -(i + 1), DiceNumberDic[-(i + 1)]);
		}

		foreach (var dice in diceList)
		{
			dice.SetActivate(false);
		}

		player.PV.RPC(nameof(player.RPC_BettingTime), RpcTarget.All, true);
	}

	public void ResetDictionary()
	{
		for (int i = 0; i < 6; i++)
		{
			pv.RPC(nameof(RPC_RefreshRolledDice), RpcTarget.All, i + 1, 0);
			pv.RPC(nameof(RPC_RefreshRolledDice), RpcTarget.All, -(i + 1), 0);
		}
	}

	[PunRPC]
	private void RPC_RefreshRolledDice(int key, int value)
	{
		DiceNumberDic[key] = value;
	}

	[PunRPC]
	private void RPC_RefreshDiceAmount(int sDiceAmount, int remainsDiceAmount)
	{
		SpecialDiceCount = sDiceAmount;
		RemainSpecialDice = remainsDiceAmount;

		diceList = FindObjectsOfType<Dice>().ToList();
		UtilClass.DebugLog($"주사위 개수 ={diceList.Count}");
	}
}