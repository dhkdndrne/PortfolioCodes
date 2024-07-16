using System;
using Bam.Singleton;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class ItemDragHandler : Singleton<ItemDragHandler>
{
	public Subject<InventoryItem> OnMouseDownSubject;
	public Func<InventoryItem, (bool isSucess, Grid targetGrid)> OnMouseUpSubject;
	private InventoryItem dragItem;
	public InventoryItem DragItem => dragItem;
	
	private void Start()
	{
		OnMouseDownSubject = new Subject<InventoryItem>();

		OnMouseDownSubject.Subscribe(item =>
		{
			dragItem = item;
			dragItem.MovePosition().Forget();

		}).AddTo(this);

		OnMouseUpSubject += item =>
		{
			dragItem = null;
			return Inventory.Instance.TryAddToInventory(item);
		};

		GameManager.Instance.Step.Where(step => step is GameStep.InventoryArrange).Subscribe(async _ =>
		{
			var gameManager = GameManager.Instance;
			var inventory = Inventory.Instance;

			while (gameManager.Step.Value == GameStep.InventoryArrange)
			{
				if (Input.GetKeyDown(KeyCode.R))
				{
					if (dragItem != null)
					{
						dragItem.Rotate();
						inventory.GetSlot(inventory.CurGrid);
					}
				}

				await UniTask.Yield();
			}
		}).AddTo(gameObject);

	}
}