using System;
using System.Collections;
using System.Collections.Generic;
using Bam.Extensions;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public abstract class InventoryItem : MonoBehaviour
{
	protected ItemSo itemSo;
	protected int[,] itemGrid;
	private List<Grid> slotGridList;

	private int angleY;
	
	private Collider col;
	private Vector3 originPos;
	private Skill skill;
	
	public HashSet<InventoryItem> NeighborItem { get; private set; }
	
	public int AngleY => angleY;
	public Vector3 OriginPos => originPos;
	public ItemSo ItemSo => itemSo;
	public int[,] ItemGrid => itemGrid;
	public List<Grid> ItemSlotGridList => slotGridList;
	public Skill Skill => skill;
	public void EnableCollider() => col.enabled = true;
	
	protected virtual void Awake()
	{
		col = GetComponent<Collider>();
		slotGridList = new List<Grid>();
		NeighborItem = new HashSet<InventoryItem>();
		InitEvent();
	}
	
	public virtual void Init(ItemSo itemSo)
	{
		this.itemSo = itemSo;
		itemGrid = itemSo.ItemGrid.Grids;
	}
	
	private void InitEvent()
	{
		transform.OnMouseDownAsObservable().Subscribe(_ =>
		{
			originPos = transform.position;
			col.enabled = false;
			ItemDragHandler.Instance.OnMouseDownSubject.OnNext(this);
		}).AddTo(gameObject);

		transform.OnMouseUpAsObservable().Subscribe(_ =>
		{
			var v = ItemDragHandler.Instance.OnMouseUpSubject.Invoke(this);

			if (v.isSucess)
			{
				SetPosition(v.targetGrid);
				EquipItem();

				if (skill == null)
				{
					skill = itemSo.GetSkillInstance();
				}
				
				Inventory.Instance.GetNeighborItems(this,v.targetGrid);
			}
			else
			{
				RaycastHit hit;
				col.enabled = true;

				if (Physics.Raycast(transform.position, transform.forward, out hit, int.MaxValue) && hit.collider.CompareTag(Define.GROUND_TAG))
				{
					transform.position = transform.position;
				}
				else
				{
					transform.position = originPos;
				}
			}
		}).AddTo(gameObject);

		GameManager.Instance.Step.Where(step => step is GameStep.Playing).Subscribe(_ =>
		{
			if (itemSo == null)
				return;
			
			if (slotGridList.Count == 0)
			{
				SellItem();
			}
		}).AddTo(gameObject);
	}

	protected abstract void EquipItem();
	public async UniTaskVoid MovePosition()
	{
		while (Input.GetMouseButton(0))
		{
			float distance = Camera.main.WorldToScreenPoint(transform.position).z;

			Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
			Vector3 objPos = Camera.main.ScreenToWorldPoint(mousePos);

			objPos.y = 0;
			transform.position = objPos;
			await UniTask.Yield();
		}
	}

	public void Rotate()
	{
		itemGrid = Extensions.RotateArray(itemGrid);

		angleY += 90;

		if (angleY > 270)
			angleY = 0;

		transform.rotation = Quaternion.Euler(90, angleY, 0);
	}

	public void RemoveGridData()
	{
		foreach (var grid in slotGridList)
		{
			Inventory.Instance.InventorySlots[grid.Y, grid.X].SetItem(null);
		}
		
		foreach (var neighbor in NeighborItem)
		{
			neighbor.NeighborItem.Remove(this);
	
			if (neighbor.HasBuffSkill())
			{
				var neighborSkill = neighbor.skill as BuffSkill;
				var weapon = neighbor as InventoryWeapon;
				
				foreach (var buff in neighborSkill.GetBuff())
					weapon.RemoveStat(buff);
				
				neighborSkill.ReduceBuffValue(this);
			}
		}

		if (HasBuffSkill())
		{
			var s = skill as BuffSkill;
			s.ResetBuff();
		}
        
		NeighborItem.Clear();
		slotGridList.Clear();
	}

	public void RollBack()
	{
		var inventory = Inventory.Instance;
		slotGridList.AddRange(inventory.TempItemData.GridList);

		foreach (var grid in slotGridList)
		{
			Inventory.Instance.AddToInventory(grid, this);
		}

		angleY = inventory.TempItemData.AngleY;
		itemGrid = inventory.TempItemData.ItemGrid;

		transform.SetPositionAndRotation(inventory.TempItemData.OriginPos, Quaternion.Euler(90, angleY, 0));
	}

	

	public void SetPosition(Grid grid)
	{
		transform.position = Inventory.Instance.GetSlotPosition(grid);
		originPos = transform.position;
	}

	public void SellItem()
	{
		PlayerData.Instance.Gold.Value += itemSo.Price;
		Inventory.Instance.UnEquipItem(this);
		
		ObjectPoolManager.Instance.Despawn(GetComponent<PoolObject>());
	}

	public bool HasBuffSkill()
	{
		return skill != null && skill.SkillType == SkillType.Buff;
	}
}