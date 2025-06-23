using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UniRx;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class TurnSystem : MonoBehaviour
{
	[field: SerializeField] public List<Player> PlayerList { get; private set; } = new();
	private List<Player> canPlayPlayerList = new();

	public Player NowPlayingPlayer { get; private set; }

	[SerializeField] private int playingPlayerIndex = -1;
	public PhotonView PV { get; private set; }

	private void Awake()
	{
		PV = GetComponent<PhotonView>();
	}

	public void SetRandomTurn()
	{
		for (int i = 0; i < 100; i++)
		{
			int index1 = Random.Range(0, PlayerList.Count);
			int index2 = Random.Range(0, PlayerList.Count);

			(PlayerList[index1], PlayerList[index2]) = (PlayerList[index2], PlayerList[index1]);
		}

		AnnouncePlayerList();
	}

	public void ClearPlayerList()
	{
		var players = FindObjectsOfType<Player>();

		foreach (var p in players)
		{
			Destroy(p.gameObject);
		}
		PlayerList.Clear();
	}
	
	[PunRPC]
	public void RPC_SetNextRoundTurn()
	{
		//처음 시작 했던 사람 다음 차례부터 시작
		//첫 플레이어를 끝으로 보내고 처음 인덱스 삭제
		PlayerList.Add(PlayerList[0]);
		PlayerList.RemoveAt(0);

		AnnouncePlayerList();

		PV.RPC(nameof(RPC_StartNextTurn), RpcTarget.MasterClient);
	}

	/// <summary>
	/// 플레이어 리스트를 추가하고
	/// 마스터가 플레이어 순서를 다른 유저에게 알림
	/// </summary>
	private void AnnouncePlayerList()
	{
		canPlayPlayerList.AddRange(PlayerList);
		var viewIDArr = PlayerList.Select(player => player.GetComponent<PhotonView>().ViewID).ToArray();

		//마스터가 섞인 순서를 다른 플레이어들에게 알려줌
		PV.RPC(nameof(RPC_SetTurnList), RpcTarget.Others, viewIDArr);
		PV.RPC(nameof(RPC_InitPlayers), RpcTarget.All);
	}

	[PunRPC]
	public void RPC_StartNextTurn()
	{
		if (NowPlayingPlayer != null && !NowPlayingPlayer.Model.CheckHasDice())
		{
			canPlayPlayerList.Remove(NowPlayingPlayer);
		}

		if (canPlayPlayerList.Count == 0)
		{
			UtilClass.DebugLog("모든 플레이어 배팅 완료");
			CasinoManager.Instance.PV.RPC(nameof(CasinoManager.Instance.RPC_CalculateCasinoPrize), RpcTarget.MasterClient);
			return;
		}

		playingPlayerIndex = playingPlayerIndex + 1 > canPlayPlayerList.Count - 1 ? 0 : playingPlayerIndex + 1;
		NowPlayingPlayer = canPlayPlayerList[playingPlayerIndex];
		NowPlayingPlayer.PV.RPC(nameof(NowPlayingPlayer.RPC_SetMyTurn), RpcTarget.All, true);
	}

	[PunRPC]
	private void RPC_SetTurnList(int[] viewIDArr)
	{
		List<Player> tempList = new();

		for (int i = 0; i < viewIDArr.Length; i++)
		{
			foreach (var player in PlayerList)
			{
				if (player.GetComponent<PhotonView>().ViewID == viewIDArr[i])
				{
					tempList.Add(player);
					break;
				}
			}
		}
		PlayerList = tempList;
		canPlayPlayerList.AddRange(PlayerList);
	}

	[PunRPC]
	private void RPC_InitPlayers()
	{
		for (int i = 0; i < PlayerList.Count; i++)
		{
			PlayerList[i].InitPlayer(i);
		}

		PhotonManager.Instance.LeftRoomSubject.Where(_ => PhotonManager.Instance.IsMaster()).Subscribe(playerNumber =>
		{
			if (PlayerList.Count == 0)
				return;
			
			PlayerList.RemoveAt(playerNumber);
			PV.RPC(nameof(RPC_StartNextTurn), RpcTarget.MasterClient);
		});
	}
}