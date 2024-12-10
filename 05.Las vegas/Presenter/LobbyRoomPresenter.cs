using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
public class LobbyRoomPresenter : MonoBehaviour
{
	private PhotonView pv;

	[Header("타이틀")]
	[SerializeField] private GameObject titleObj;

	[Header("로비")]
	[SerializeField] private GameObject roomObj;
	[SerializeField] private Image[] playerPanels;
	[SerializeField] private GameObject[] playerReadyTexts;
	[SerializeField] private TextMeshProUGUI roomName;
	[SerializeField] private Button readyBtn;
	[SerializeField] private Button exitBtn;

	public bool[] readyPlayers = new bool[4];
	private void Start()
	{
		pv = GetComponent<PhotonView>();

		exitBtn.onClick.AddListener(() =>
		{
			ResetLobbyUI();
			roomObj.SetActive(false);
			titleObj.SetActive(true);
			PhotonNetwork.LeaveRoom();
		});

		readyBtn.onClick.AddListener(() =>
		{
			//레디 버튼눌렀을때 방장이면 시작 아니면 레디
			if (PhotonManager.Instance.IsMaster())
			{
				if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
				{
					int count = 0;
					for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
					{
						if (!readyPlayers[i]) count++;
					}

					if (count == 1) PlayGame();
				}
			}
			else
			{
				int num = PhotonNetwork.LocalPlayer.GetPlayerNumber();
				pv.RPC(nameof(RPC_Ready), RpcTarget.AllBuffered, !readyPlayers[num], num);
			}
		});

		PhotonManager.Instance.LeftRoomSubject.Subscribe(index =>
		{
			if(PhotonManager.Instance.IsMaster()) PhotonNetwork.CleanRpcBufferIfMine(pv);
			
			if (!GameManager.Instance.Model.isGameStarted)
			{
				ChangeIndex(index);
				roomObj.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = PhotonManager.Instance.IsMaster() ? "시작" : "준비";
			}
		}).AddTo(gameObject);

		PhotonManager.Instance.JoinRoomSubject.Subscribe(value =>
		{
			roomName.text = PhotonNetwork.CurrentRoom.Name;

			titleObj.SetActive(false);
			roomObj.SetActive(true);

			roomObj.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = PhotonManager.Instance.IsMaster() ? "시작" : "준비";
			pv.RPC(nameof(RPC_RefreshJoinedPlayer), RpcTarget.AllBuffered, value.Item1, value.Item2);
		}).AddTo(gameObject);

		// PhotonManager.Instance.MasterChangedSubject.Subscribe(value =>
		// {
		// 	roomName.text = value.Item2;
		// 	ChangeIndex(value.Item1);
		//
		// 	roomObj.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = PhotonManager.Instance.IsMaster() ? "시작" : "준비";
		// }).AddTo(gameObject);
		
	}

	public void ActivateRoomObject()
	{
		roomObj.gameObject.SetActive(true);
	}

	/// <summary>
	/// 나간 플레이어의 인덱스를 기준으로 인덱스 다시 설정
	/// </summary>
	/// <param name="index"></param>
	private void ChangeIndex(int index)
	{
		var myIndex = PhotonNetwork.LocalPlayer.GetPlayerNumber();
		bool isLowerIndex = true;

		if (myIndex > index)
		{
			PhotonNetwork.LocalPlayer.SetPlayerNumber(myIndex - 1);
			isLowerIndex = false;
		}

		ResetLobbyUI();

		GetChangedPlayerNum(myIndex == 0 ? 0 : myIndex - 1, isLowerIndex).Forget();
	}

	private void ResetLobbyUI()
	{
		for (int i = 0; i < readyPlayers.Length; i++)
		{
			readyPlayers[i] = false;
			playerPanels[i].gameObject.SetActive(false);
			playerPanels[i].color = Color.black;
			playerReadyTexts[i].SetActive(false);
		}
	}
	
	/// <summary>
	/// 플레이어 인덱스 다시 설정 후 바뀔때까지 대기
	/// </summary>
	/// <param name="index"></param>
	/// <param name="isLowerIndex"></param>
	private async UniTaskVoid GetChangedPlayerNum(int index, bool isLowerIndex)
	{
		await UniTask.WaitUntil(() => isLowerIndex || index == PhotonNetwork.LocalPlayer.GetPlayerNumber());
		PhotonNetwork.LocalPlayer.NickName = $"Player {PhotonNetwork.LocalPlayer.GetPlayerNumber()}";

		for (int i = 0; i > PhotonNetwork.CurrentRoom.PlayerCount; i++)
		{
			playerPanels[index].gameObject.SetActive(true);
		}
		pv.RPC(nameof(RPC_RefreshJoinedPlayer), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.GetPlayerNumber(), PhotonNetwork.LocalPlayer.NickName);
	}

	/// <summary>
	/// 게임 시작
	/// </summary>
	private void PlayGame()
	{
		pv.RPC(nameof(RPC_PlayGame), RpcTarget.All);
		PhotonNetwork.CurrentRoom.IsOpen = false;
		StartCoroutine(Co_InitGame());
	}

	private IEnumerator Co_InitGame()
	{
		yield return new WaitForSeconds(1f);
		GameManager.Instance.InitGame().Forget();
	}

	/// <summary>
	/// 레디상태에 따라 UI변경
	/// </summary>
	/// <param name="isReady"></param>
	/// <param name="index"></param>
	[PunRPC]
	private void RPC_Ready(bool isReady, int index)
	{
		readyPlayers[index] = isReady;
		playerPanels[index].color = isReady ? Color.green : Color.black;
		playerReadyTexts[index].SetActive(isReady);
	}

	[PunRPC]
	private void RPC_PlayGame()
	{
		roomObj.SetActive(false);
		PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);

		for (int i = 0; i < readyPlayers.Length; i++)
		{
			pv.RPC(nameof(RPC_Ready), RpcTarget.AllBuffered, false, i);
		}

	}

	/// <summary>
	/// 방에 들어와있는 유저 상태 업데이트
	/// </summary>
	/// <param name="index"></param>
	/// <param name="nickName"></param>
	[PunRPC]
	private void RPC_RefreshJoinedPlayer(int index, string nickName)
	{
		playerPanels[index].gameObject.SetActive(true);
		playerPanels[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = index == PhotonNetwork.LocalPlayer.GetPlayerNumber() ? nickName + " (me)" : nickName;
	}
}