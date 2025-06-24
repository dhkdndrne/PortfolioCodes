using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Unit : MonoBehaviour, IDamageable
{
	protected string unitName;
	
	[SerializeField] protected Attribute attribute;
	protected Dictionary<Type, List<Buff>> activeBuffsDict = new Dictionary<Type, List<Buff>>();
	
	private event Action onDeath;
	public event Action OnDeath
	{
		add { onDeath += value; }
		remove { onDeath -= value; }
	}

	protected virtual void OnEnable()
	{
		onDeath += OnUnitDead;
	}
	protected virtual void OnDisable()
	{
		onDeath -= OnUnitDead;
	}
	public string UnitName => unitName;
	
	public abstract void Init();

	public abstract bool IsDead();

	protected void InvokeOnDeath() // 자식 클래스에서 호출할 수 있도록 보호된 메서드 제공
	{
		RemoveAllBuffs();
		onDeath?.Invoke();
		gameObject.SetActive(false);
	}

	protected abstract void OnUnitDead();
	
	public abstract void Hit(Unit attacker);
	
	public bool Heal(float amount)
	{
		if (IsDead()) return false;
		if (Mathf.Approximately(attribute.HpRatio.Value, 1f)) return false;
		
		attribute.Hp.Value += amount;
		return true;
	}
	
	public void AdjustShieldValue(float shieldAmount)
	{
		attribute.Shield.Value += shieldAmount;
		// 음수 값이 들어가지 않도록 보정
		if(attribute.Shield.Value < 0)
			attribute.Shield.Value = 0;
	}
	
	public void AddBuff(Buff buff)
	{
		Type key = buff.GetType();
		if (!activeBuffsDict.TryGetValue(key, out List<Buff> buffList))
		{
			buffList = new List<Buff>();
			activeBuffsDict[key] = buffList;
		}
		buffList.Add(buff);
		buff.Apply(this);
	}

	public void RemoveBuff(Buff buff)
	{
		Type key = buff.GetType();
		if (activeBuffsDict.TryGetValue(key, out List<Buff> buffList))
		{
			buffList.Remove(buff);
			if (buffList.Count == 0)
			{
				activeBuffsDict.Remove(key);
			}
		}
		buff.Remove(this);
	}
	protected void RemoveAllBuffs()
	{
		// 현재 걸려 있는 모든 버프를 한 리스트에 모아서 순회
		var allBuffs = activeBuffsDict.Values.SelectMany(list => list).ToList();
		foreach (var buff in allBuffs)
		{
			// 하나씩 제거
			RemoveBuff(buff);
		}
		activeBuffsDict.Clear();
	}
	public float GetFinalDamage()
	{
		// {(캐릭터 기초 공격력) x (합연산 퍼센테이지의 총합) + 격려 } x (곱연산 퍼센테이지)
		return attribute.AttackPower *  (1 + attribute.GetAddTotalExtraAttribute(AttributeType.Damage)) * attribute.GetMulTotalExtraAttribute(AttributeType.Damage);
	}
	public float GetFinalMeleeDefense()
	{
		// 최종 물리 방어력={(적 기초 물리방어력)+(방어력 버프)}*(1-방깎 퍼센테이지)*(1-방깎 퍼센테이지)*… (복리계산)
		
		float defense = attribute.Defense + attribute.GetAddTotalExtraAttribute(AttributeType.Defense);
		float decrease = attribute.GetMulTotalExtraAttribute(AttributeType.Defense);
	
		return defense * decrease;
	}
	
	public void SetAdditionalAttribute(AttributeModifier attribute)
	{
		this.attribute.SetAdditionalAttribute(attribute);
	}
	public void RemoveAdditionalAttribute(AttributeModifier attribute)
	{
		this.attribute.RemoveAdditionalAttribute(attribute);
	}
}