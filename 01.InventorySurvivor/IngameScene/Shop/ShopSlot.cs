using System;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class ShopSlot : MonoBehaviour
{
	[SerializeField] private Image itemIcon;
	[SerializeField] private Image border;
	[SerializeField] private Button buyButton;
	[SerializeField] private TextMeshProUGUI priceText;
	[SerializeField] private GameObject lockImage;
	
	[SerializeField] private Transform gridHolder;
	private GameObject[,] gridImageArr;

	private ItemSo itemSo;
	private Transform spawnPoint;

	public bool IsLock { get; private set; }
	private bool isBought;
	private bool isOnMouse;

	private void Awake()
	{
		Init();
	}

	public void Init(Transform spawnPoint)
	{
		this.spawnPoint = spawnPoint;
		IsLock = false;
		isBought = false;
	}

	private void Init()
	{
		gridImageArr = new GameObject[ITEM_GRID_MAX_ROW, ITEM_GRID_MAX_COL];

		for (int y = 0; y < ITEM_GRID_MAX_ROW; y++)
		{
			for (int x = 0; x < ITEM_GRID_MAX_COL; x++)
			{
				gridImageArr[y, x] = gridHolder.transform.GetChild(x + (y * ITEM_GRID_MAX_COL)).gameObject;
			}
		}

		buyButton.onClick.AddListener(() =>
		{
			if (itemSo.Price <= PlayerData.Instance.Gold.Value && !isBought)
			{
				PlayerData.Instance.Gold.Value -= itemSo.Price;
				isBought = true;

				ObjectPoolManager.Instance.CheckAlreadyExist($"{itemSo.ID}_inventory", itemSo.InventoryItem.gameObject);
				var itemObj = ObjectPoolManager.Instance.Spawn($"{itemSo.ID}_inventory");
				itemObj.transform.SetPositionAndRotation(spawnPoint.position, Quaternion.Euler(90, 0, 0));
				
				var item = itemObj.GetComponent<InventoryItem>();
				item.Init(itemSo);
				
				priceText.text = "구매 완료";
				lockImage.SetActive(false);
				IsLock = false;
				gridHolder.gameObject.SetActive(false);
				itemIcon.enabled = false;
				buyButton.enabled = false;
			}
		});

		var mouseEnterStream = itemIcon.OnPointerEnterAsObservable();
		var mouseExitStream = itemIcon.OnPointerExitAsObservable();
		
		mouseEnterStream.SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(ITEM_INFO_ONMOUSE_TICK)))
			.TakeUntil(mouseExitStream)
			.Where(_ => gameObject.activeSelf && !isBought)
			.RepeatUntilDestroy(gameObject)
			.Subscribe(_ =>
			{
				isOnMouse = true;
				PopUpManager.Instance.ShowShopItemInfo(itemSo);
			})
			.AddTo(this);

		mouseExitStream.Timestamp()
			.Zip(mouseEnterStream.Timestamp(), (d, u) => (u.Timestamp - d.Timestamp).TotalMilliseconds / 1000.0f)
			.Where(time => time < ITEM_INFO_ONMOUSE_TICK);
        
		mouseExitStream.Subscribe(_ =>
		{
			isOnMouse = false;
			PopUpManager.Instance.DisableItemInfo();
		}).AddTo(this);

		Observable.EveryUpdate().Where(_ => isOnMouse).Subscribe(_ =>
		{
			if (Input.GetMouseButtonDown(1))
			{
				IsLock = !IsLock;
				lockImage.SetActive(IsLock);
			}
		}).AddTo(this);
	}
	
	public void SetSlot(ItemSo itemSo, Shop shop)
	{
		this.itemSo = itemSo;

		itemIcon.enabled = true;
		buyButton.enabled = true;

		itemIcon.sprite = itemSo.Sprite;
		priceText.text = $"{itemSo.Price}g";
		border.sprite = shop.GetSlotBorderSprite(itemSo.Rarity);

		isBought = false;
		ShowGridUI(itemSo);
	}

	private void ShowGridUI(ItemSo itemSo)
	{
		gridHolder.gameObject.SetActive(true);
		for (int y = 0; y < ITEM_GRID_MAX_ROW; y++)
		{
			for (int x = 0; x < ITEM_GRID_MAX_COL; x++)
			{
				gridImageArr[y, x].SetActive(itemSo.ItemGrid.Grids[y, x] == 1);
			}
		}
	}
}