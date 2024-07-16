using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
	protected Animator animator;
	
	private AttackItemSo itemSo;
	public AttackItemSo ItemSo => itemSo;
	private PoolObject poolObject;

	private bool canAttack;
	protected float coolTime;

	private void Awake()
	{
		poolObject = GetComponent<PoolObject>();
		animator = GetComponent<Animator>();
	}
	private bool CanAttack => canAttack && coolTime == 0;
	protected abstract void Attack();
	
	public virtual void UpdateCoolTime(float deltaTime)
	{
		coolTime -= deltaTime;

		if (coolTime <= 0)
			coolTime = 0;
	}
	protected abstract void OnTriggerEnter(Collider other);

	public void Init(ItemSo itemSo)
	{
		this.itemSo = itemSo as AttackItemSo;
		coolTime = 0f;
	}

	public void DeSpawn()
	{
		ObjectPoolManager.Instance.Despawn(poolObject);
	}
}