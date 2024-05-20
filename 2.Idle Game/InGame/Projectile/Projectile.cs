using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	private PoolObject poolObject;

	protected UnitAI caster;
	protected UnitHp target;

	protected double damage;
	protected float speed;

	public virtual void Init(UnitAI caster,UnitHp target, double damage, float speed)
	{
		this.target = target;
		this.caster = caster;
		this.damage = damage;
		this.speed = speed;
	}

	protected virtual void Awake()
	{
		poolObject = GetComponent<PoolObject>();
	}

	protected virtual async UniTaskVoid Move(){}

	protected void Despawn()
	{
		ObjectPoolManager.Instance.Despawn(poolObject);
	}
}