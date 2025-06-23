using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bam.Extensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
	//UI
	[SerializeField] private Sprite[] slotBorderSprites;
	[SerializeField] private Button tabButton;
	[SerializeField] private Button rerollButton;

	[SerializeField] private TextMeshProUGUI rerollCostText;
	[SerializeField] private TextMeshProUGUI goldText;

	[SerializeField] private RectTransform panel;

	//오브젝트
	[SerializeField] private ShopSlot[] slots;
	//[SerializeField] private Transform[] spawnPoints;

	private bool isOpenShop;
	private int rerollCost;

	private void Awake()
	{
		for (int i = 0; i < slots.Length; i++)
			slots[i].Init(Inventory.Instance.InventoryUnEquipedItemHolder.GetSpawnPoint(i));
	}

	private void Start()
	{
		var gameManger = GameManager.Instance;

		tabButton.onClick.AddListener(() =>
		{
			if (gameManger.Step.Value is GameStep.InventoryArrange)
				MovePanel().Forget();
		});

		rerollButton.onClick.AddListener(() =>
		{
			if (gameManger.Step.Value is GameStep.InventoryArrange && rerollCost <= PlayerData.Instance.Gold.Value)
			{
				PlayerData.Instance.Gold.Value -= rerollCost;
				SetSlotItem();
			}
		});

		gameManger.Step.Subscribe(step =>
		{
			switch (step)
			{
				case GameStep.UnLockSlot:
					//인벤토리 모드 들어갈때 한번 초기화
					goldText.text = PlayerData.Instance.Gold.Value.ToString();
					rerollCostText.text = $"다시 굴리기 <color=#FFEA00>{Inventory.Instance.rerollCost}g</color>";
					break;

				case GameStep.InventoryArrange:
					SetSlotItem();
					MovePanel().Forget();
					break;
			}

		}).AddTo(this);

		// 인벤토리 모드일때 지속해서 바뀌게 (나중에 한번 손봐보자)
		PlayerData.Instance.Gold.Where(_ => gameManger.Step.Value is not GameStep.Playing).Subscribe(gold =>
		{
			goldText.text = gold.ToString();
		}).AddTo(this);
	}

	private void SetSlotItem()
	{
		for (int i = 0; i < slots.Length; i++)
		{
			if (!slots[i].IsLock)
			{
				slots[i].SetSlot(ItemManager.Instance.GetRandomItem(), this);
			}
		}
	}

	public Sprite GetSlotBorderSprite(ItemRarity rariy)
	{
		return slotBorderSprites[(int)rariy];
	}

	private async UniTaskVoid MovePanel()
	{
		await panel.DOAnchorPosX(isOpenShop ? -panel.sizeDelta.x : 0, .3f);
		isOpenShop = !isOpenShop;
	}
}