using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bam.Extensions;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class Casino : MonoBehaviour
{

	[SerializeField] private Transform moneyPos; //돈 놓을 위치
	[field: SerializeField] public int CasinoNum { get; private set; }

	public List<Money> PrizeList { get; private set; } = new();   // 카지노에 걸린 상금 넣어둘 큐
	private Dictionary<string, int> bettingDiceDictionary = new() // 카지노에 배팅한 주사위 개수를 알기위한 딕셔너리
	{
		{ "Player 0", 0 },
		{ "Player 1", 0 },
		{ "Player 2", 0 },
		{ "Player 3", 0 },
		{ "Special", 0 }
	};
	public List<KeyValuePair<string, int>> SortedList { get; private set; }

	private PhotonView pv;
	public PhotonView PV => pv;

	private void Awake()
	{
		pv = GetComponent<PhotonView>();
		RemovePlayerKeyAndInitList().Forget();
	}

	private async UniTaskVoid RemovePlayerKeyAndInitList()
	{
		await UniTask.WaitUntil(() => GameManager.Instance.Model.isGameStarted);

		var playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
		for (int i = 4; i >= playerCount; i--)
		{
			bettingDiceDictionary.Remove($"Player {i}");
		}
	}

	public void SetPrize(List<Money> prizeList)
	{
		prizeList = prizeList.OrderByDescending(n => n.MoneyData.Price).ToList();

		Vector3 v3TargetPos = moneyPos.position;
		Vector3 v3Offset = new Vector3(0, 0, -3.285f);
		int index = 0;

		foreach (var prize in prizeList)
		{
			PrizeList.Add(prize);
			prize.ChangeSortingOrder(index);
			prize.MoveToPostition(v3TargetPos + (v3Offset * index++));
		}
	}

	[PunRPC]
	public void RPC_BetDice(string playerID, int diceAmount, int sDiceAmount)
	{
		if (!playerID.IsNullOrWhitespace())
			bettingDiceDictionary[playerID] += diceAmount;

		bettingDiceDictionary["Special"] += sDiceAmount;

		SortedList = new List<KeyValuePair<string, int>>(bettingDiceDictionary);
		SortedList.Sort((x, y) => y.Value.CompareTo(x.Value));
	}

	/// <summary>
	/// 배팅 개수 같은 플레이어 삭제
	/// </summary>
	[PunRPC]
	public void RPC_DeleteEqualValuePlayer()
	{
		int tempValue = int.MinValue;
		SortedList = SortedList.Where(x =>
		{
			if (tempValue == x.Value || x.Value == 0)
				return false;

			tempValue = x.Value;
			return true;
		}).ToList();

		int index = 1;
		foreach (var player in SortedList)
		{
			UtilClass.DebugLog($"카지노 {CasinoNum} \n {index++}등 : {player.Key} / 배팅한 주사위 개수 {player.Value}",Define.LogType.Warning);
		}
	}

	[PunRPC]
	public void RPC_GivePrize()
	{
		for (int i = 0; i < PrizeList.Count; i++)
		{
			if (i == SortedList.Count)
				break;
			
			if (!SortedList[i].Key.Equals("Special"))
			{
				var number = int.Parse(SortedList[i].Key.Split(' ')[1]); //플레이어 숫자만
				Player player = GameManager.Instance.TurnSystem.PlayerList.Where(p => p.Model.PlayerNumber == number).First();
				
				player.PV.RPC(nameof(player.RPC_GetMoney), RpcTarget.All, PrizeList[i].MoneyData.Price);
				UtilClass.DebugLog($"{i +1}등  {SortedList[i].Key}에게 {PrizeList[i].MoneyData.Price} 추가");
			}
		}
	}

	[PunRPC]
	public void RPC_DeleteMoneyCard()
	{
		foreach (var moneyCard in PrizeList)
		{
			moneyCard.gameObject.SetActive(false);
		}

		bettingDiceDictionary = bettingDiceDictionary.ToDictionary(x => x.Key, x => 0);
		
		PrizeList?.Clear();
		SortedList?.Clear();
	}
}