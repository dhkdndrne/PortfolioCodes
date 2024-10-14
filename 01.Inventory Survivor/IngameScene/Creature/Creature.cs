using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Creature : MonoBehaviour,IHitable
{
	protected float hp;
	protected float maxHp;
	
	public abstract void Hit(DamageType damageType,float damage);
	protected abstract void Dead();
}
