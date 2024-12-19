using System;
using Bam.Extensions;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using static Define;
public class InventorySlot : MonoBehaviour
{
	[SerializeField] private GameObject canUnlockFX;

	private MeshRenderer meshRenderer;
	private MaterialPropertyBlock mpb;

	private Grid grid;
	private InventoryItem item;
	private InventoryItem tempItem;

	private bool isLock;
	private bool canUnlock;

	public bool IsLock => isLock;
	public InventoryItem Item => item;
	public Grid Grid => grid;
	private void Awake()
	{
		GetComponents();
	}

	private void GetComponents()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		mpb = new MaterialPropertyBlock();
	}

	public void EnableFX()
	{
		canUnlockFX.SetActive(true);
		canUnlock = true;

		canUnlockFX.transform.DOKill();
		canUnlockFX.transform.localScale = Vector3.zero;
		canUnlockFX.transform.DOScale(1, 0.2f);
	}

	public void DisableFX()
	{
		canUnlock = false;
		canUnlockFX.transform.DOKill();
		canUnlockFX.transform.DOScale(0, 0.2f).OnComplete(() => canUnlockFX.SetActive(false));
	}

	public void Init(int x, int y, Subject<Grid> onMouseSubject, bool isLock)
	{
		grid = new Grid(x, y);
		this.isLock = isLock;

		if (this.isLock)
			ChangeColor(new Color(0.3f, 0.3f, 0.3f, 0.1f));

		transform.OnMouseEnterAsObservable().Where(_ => !this.isLock && !Extensions.IsPointerOverUI()).Subscribe(_ =>
		{
			onMouseSubject.OnNext(grid);
		}).AddTo(this);

		transform.OnMouseEnterAsObservable()
			.SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(ITEM_INFO_ONMOUSE_TICK)))
			.TakeUntil(transform.OnMouseExitAsObservable())
			.Where(_ => item != null && !Extensions.IsPointerOverUI())
			.RepeatUntilDestroy(gameObject)
			.Subscribe(_ =>
			{
				PopUpManager.Instance.ShowInventoryItemInfo(item);
			})
			.AddTo(this);

		transform.OnMouseExitAsObservable().Where(_ => !this.isLock && !Extensions.IsPointerOverUI()).Subscribe(_ =>
		{
			onMouseSubject.OnNext(null);
			PopUpManager.Instance.DisableItemInfo();
		}).AddTo(this);

		transform.OnMouseDownAsObservable().Subscribe(_ =>
		{
			if (this.isLock)
			{
				if (GameManager.Instance.Step.Value is GameStep.UnLockSlot && canUnlock)
				{
					UnLock();
					onMouseSubject.OnNext(grid);
				}
			}
			else
			{
				if (item != null)
				{
					tempItem = item;
					Inventory.Instance.SaveBeforeData(tempItem);
					item.RemoveGridData();

					ItemDragHandler.Instance.OnMouseDownSubject.OnNext(tempItem);
					PopUpManager.Instance.DisableItemInfo();
				}
			}
		}).AddTo(gameObject);

		transform.OnMouseUpAsObservable().Where(_ => !this.isLock).Subscribe(_ =>
		{
			if (tempItem == null)
				return;

			var v = ItemDragHandler.Instance.OnMouseUpSubject.Invoke(tempItem);

			if (v.isSucess)
			{
				tempItem.SetPosition(v.targetGrid);
			}
			else
			{
				RaycastHit hit;
				if (Physics.Raycast(tempItem.transform.position, tempItem.transform.forward, out hit, int.MaxValue) &&
				    hit.collider.CompareTag(GROUND_TAG))
				{
					var temp = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
						Input.mousePosition.y, -Camera.main.transform.position.z));
					temp.y = 0;

					tempItem.transform.position = temp;
					tempItem.EnableCollider();
					Inventory.Instance.UnEquipItem(tempItem);
					return;
				}

				tempItem.RollBack();
			}

			if (v.targetGrid != null)
				Inventory.Instance.GetNeighborItems(tempItem, v.targetGrid);
			
			tempItem = null;
		}).AddTo(gameObject);
	}

	private void UnLock()
	{
		isLock = false;
		ChangeColor(Color.white);
		Inventory.Instance.UnLockSlot(grid);
	}
	public void ChangeColor(in Color color)
	{
		Extensions.ChangeMeshColor(meshRenderer, mpb, color, "_BaseColor");
	}

	public void SetItem(InventoryItem item)
	{
		this.item = item;
		Color color = item == null ? Color.white : ColorManager.Instance.GetItemRarityColor(item.ItemSo.Rarity);
		ChangeColor(color);
	}
}