using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class IngamePresenter : MonoBehaviour
{
	[SerializeField] private Button rollDiceBtn;
	[SerializeField] private List<Transform> diceUIList = new();
	[SerializeField] private GameObject diceUI;
	[SerializeField] private GameObject myTurnTextObject;

	[SerializeField] private TextMeshProUGUI diceAmountText;
	[SerializeField] private TextMeshProUGUI sDiceAmountText;

	[SerializeField] private RectTransform roundBgRectTransform;
	[SerializeField] private TextMeshProUGUI roundText;

	[Header("카지노 정보창")]
	[SerializeField] private Transform casinoInfoObject;
	[SerializeField] private TextMeshProUGUI casinoNumText;
	[SerializeField] private TextMeshProUGUI prizeText;
	[SerializeField] private TextMeshProUGUI bettingStateText;
	[field: SerializeField] public List<TextMeshProUGUI> PlayersPrizeList { get; private set; } = new();

	[Header("게임 결과창")]
	[SerializeField] private GameObject gameResultPanel;
	[SerializeField] private List<TextMeshProUGUI> playerResultList;

	public Action DiceRollAction;
	public PhotonView PV { get; private set; }
	private ReactiveProperty<Casino> selectedCasino = new();
	private StringBuilder sb = new();

	private bool isPanelMoved;
	private float elapse;

	private void Start()
	{
		PV = GetComponent<PhotonView>();

		rollDiceBtn.onClick.AddListener(() =>
		{
			rollDiceBtn.gameObject.SetActive(false);
			DiceRollAction.Invoke();
		});

		// 매프레임 체크하면서 카지노 위에 마우스가 올라갔는지 체크 후 카지노에 마우스가 올라가있으면 카지노 정보 출력
		Observable.EveryUpdate().Where(_ => GameManager.Instance.Model.isGameStarted).Subscribe(_ =>
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hits = Physics.RaycastAll(ray);

			foreach (var hit in hits)
			{
				if (!EventSystem.current.IsPointerOverGameObject() && hit.transform.TryGetComponent(out Casino casino))
				{
					if (casino != selectedCasino.Value)
					{
						selectedCasino.Value = casino;
						casinoInfoObject.transform.DOKill();
						casinoInfoObject.transform.DOMoveX(0, .5f);
					}
					isPanelMoved = true;
					elapse = 0;
					break;
				}

				isPanelMoved = false;
			}

			if (!isPanelMoved && selectedCasino.Value != null)
			{
				elapse += Time.deltaTime;

				if (elapse >= 0.3f)
				{
					selectedCasino.Value = null;
					casinoInfoObject.transform.DOKill();
					casinoInfoObject.transform.DOMoveX(-600, .5f);
					elapse = 0f;
				}
			}
		}).AddTo(gameObject);

		selectedCasino.Where(casino => casino != null).Subscribe(casino =>
		{
			casinoNumText.text = $"카지노 {casino.CasinoNum}";

			sb.Clear();
			for (int i = 0; i < casino.PrizeList.Count; i++)
			{
				sb.Append($"{i + 1}등 상금 : {casino.PrizeList[i].MoneyData.Price} \n");
			}
			prizeText.text = sb.ToString();

			sb.Clear();

			if (casino.SortedList != null)
			{
				int rank = 1;
				foreach (var value in casino.SortedList)
				{
					if (value.Value > 0)
					{
						sb.Append(rank switch
						{
							1 => "1st",
							2 => "2nd",
							3 => "3rd",
							_ => $"{rank}th"
						});

						sb.Append($": {value.Key} => {value.Value} \n");
						rank++;
					}
				}
			}

			bettingStateText.text = sb.ToString();

		}).AddTo(gameObject);


		GameManager.Instance.GameOverAction += () =>
		{
			gameResultPanel.gameObject.SetActive(true);
			var list = GameManager.Instance.TurnSystem.PlayerList;

			foreach (var player in playerResultList)
				player.gameObject.SetActive(false);
			
			for (int i = 0; i < list.Count; i++)
			{
				string temp = PhotonNetwork.LocalPlayer.GetPlayerNumber() == list[i].Model.PlayerNumber ? "(me)" : " ";
				playerResultList[i].gameObject.SetActive(true);
				playerResultList[i].text = $"{i + 1}등 Player{list[i].Model.PlayerNumber} {temp}";
			}
		};
	}

	public void InitUIEvent(Player player)
	{
		GameManager.Instance.Model.Round.Subscribe(round =>
		{
			roundText.text = $"Round {round}";
		}).AddTo(gameObject);

		player.Model.IsMyTurn.Subscribe(value =>
		{
			rollDiceBtn.gameObject.SetActive(value);
			myTurnTextObject.SetActive(value);
		}).AddTo(gameObject);

		player.Model.IsBettingTime.Subscribe(value =>
		{
			if (!value)
				PV.RPC(nameof(RPC_TurnOffDiceUI), RpcTarget.All);
		}).AddTo(gameObject);

		player.Model.Dice.Where(_ => player.PV.IsMine).Subscribe(value =>
		{
			diceAmountText.text = $"나의 주사위 개수 : {value}";
		}).AddTo(gameObject);

		player.Model.SpecialDice.Where(_ => player.PV.IsMine).Subscribe(value =>
		{
			sDiceAmountText.text = $"나의 특수 주사위 개수 : {value}";
		}).AddTo(gameObject);


		PhotonManager.Instance.LeftRoomSubject.Subscribe(playerNumber =>
		{
			if (player.PV.IsMine)
			{
				PlayersPrizeList[playerNumber].gameObject.SetActive(false);
			}
		});

		// 플레이어 상금 텍스트 오브젝트 켜줌
		// 플레이어들의 상금 텍스트 표시
		PlayersPrizeList[player.Model.PlayerNumber].gameObject.SetActive(true);
		player.Model.Money.Subscribe(value =>
		{
			PlayersPrizeList[player.Model.PlayerNumber].text = $"플레이어{player.Model.PlayerNumber} 상금 : {value}";
		}).AddTo(gameObject);
	}

	public void ShowDiceUI(int dot, int diceAmount, int sDiceAmount) => PV.RPC(nameof(RPC_ShowDiceUI), RpcTarget.All, dot, diceAmount, sDiceAmount);

	[PunRPC]
	private void RPC_ShowDiceUI(int dot, int diceAmount, int sDiceAmount)
	{
		if (diceAmount == 0 && sDiceAmount == 0) return;

		diceUI.SetActive(true);

		if (!diceUIList[dot].gameObject.activeSelf)
			diceUIList[dot].gameObject.SetActive(true);

		for (int i = 0; i < diceAmount; i++)
			diceUIList[dot].GetChild(i).gameObject.SetActive(true);

		for (int j = 0; j < sDiceAmount; j++)
			diceUIList[dot].GetChild(diceUIList[dot].childCount - (j + 1)).gameObject.SetActive(true);
	}

	[PunRPC]
	private void RPC_TurnOffDiceUI()
	{
		foreach (var diceUIObj in diceUIList)
		{
			for (int i = 0; i < diceUIObj.childCount; i++)
			{
				diceUIObj.transform.GetChild(i).gameObject.SetActive(false);
			}

			diceUIObj.gameObject.SetActive(false);
		}

		diceUI.SetActive(false);
	}

	public async UniTaskVoid ShowRoundAnimation()
	{
		roundBgRectTransform.gameObject.SetActive(true);
		await roundBgRectTransform.DOAnchorPosY(0, .8f).SetEase(Ease.Linear);
		await UniTask.Delay(1000);
		await roundBgRectTransform.DOAnchorPosY(roundBgRectTransform.sizeDelta.y, .8f).SetEase(Ease.Linear);
		roundBgRectTransform.gameObject.SetActive(false);
	}
}