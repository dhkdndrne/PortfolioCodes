using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class PhotonManager : MonoBehaviourPunCallbacks
{
	private static PhotonManager instance;
	public static PhotonManager Instance { get => instance; }

	private PhotonView pv;

	public Subject<int> LeftRoomSubject { get; private set; } = new();
	public Subject<(int, string)> JoinRoomSubject { get; private set; } = new();
	public Subject<(int, string)> MasterChangedSubject { get; private set; } = new();

	private void Awake()
	{
		instance = this;
		pv = GetComponent<PhotonView>();
		//Screen.SetResolution(1920, 1080, false);
		Screen.SetResolution(800, 600, false);

	}

	public void OnBtn_EnterRoom()
	{
		PhotonNetwork.ConnectUsingSettings();
	}

	public override void OnLeftRoom()
	{
		PhotonNetwork.Disconnect();
	}
	/// <summary>
	/// 랜덤 방 참가에 실패했을때
	/// </summary>
	/// <param name="returnCode"></param>
	/// <param name="message"></param>
	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		PhotonNetwork.LocalPlayer.NickName = "Player 0";
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = 4;
		roomOptions.CleanupCacheOnLeave = false;

		PhotonNetwork.JoinOrCreateRoom($"방 {Random.Range(1, 101)}", roomOptions,null);
	}

	public override void OnConnectedToMaster()
	{
		PhotonNetwork.JoinRandomRoom();
	}

	/// <summary>
	/// 방에 참가했을때
	/// </summary>
	public override void OnJoinedRoom()
	{
		var index = PhotonNetwork.CurrentRoom.PlayerCount - 1;
		PhotonNetwork.LocalPlayer.SetPlayerNumber(index);
		PhotonNetwork.LocalPlayer.NickName = $"Player {index}";
		GetPlayerNum().Forget();
	}

	private async UniTaskVoid GetPlayerNum()
	{
		await UniTask.WaitUntil(() => PhotonNetwork.LocalPlayer.GetPlayerNumber() != -1);
		JoinRoomSubject.OnNext((PhotonNetwork.LocalPlayer.GetPlayerNumber(), PhotonNetwork.LocalPlayer.NickName));
	}

	public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
	{
		UtilClass.DebugLog($"마스터 바뀜 {newMasterClient.NickName}");
		MasterChangedSubject.OnNext((newMasterClient.GetPlayerNumber(), $"{newMasterClient.NickName} 의 방"));
	}

	public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
	{
		LeftRoomSubject.OnNext(otherPlayer.GetPlayerNumber());
	}

	public bool IsMaster() => PhotonNetwork.LocalPlayer.IsMasterClient;
}