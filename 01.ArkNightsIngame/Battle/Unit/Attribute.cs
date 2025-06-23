using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.Serialization;
using Bam.Extensions;

[System.Serializable]
public class Attribute
{
	public ObservableValue<float> Hp { get; private set; }
	public ObservableValue<float> MaxHp { get; private set; }
	public ObservableValue<float> HpRatio { get; private set; } = new ObservableValue<float>(1);
	public ObservableValue<float> Shield { get; private set; } = new ObservableValue<float>(0);
	public ObservableValue<float> ShieldRatio { get; private set; } = new ObservableValue<float>(0);
	
	[SerializeField] protected float attackSpeed;
	[SerializeField] protected float attackPower;

	[SerializeField] protected float defense;
	[SerializeField] protected float magicResistance;
	
	private AttributeModifierCollection attributeModifier = new AttributeModifierCollection();
	
	public float AttackSpeed => attackSpeed;
	public float AttackPower => attackPower;
	public float Defense => defense;
	public float MagicResistance => magicResistance + attributeModifier.GetAddTotal(AttributeType.MagicResistance);
	
	public Attribute(float maxHp, float attackSpeed, float atkPower, float defense, float magicResistance)
	{
		MaxHp = new ObservableValue<float>(maxHp);
		Hp = new ObservableValue<float>(maxHp);
		this.attackSpeed = attackSpeed;
		this.attackPower = atkPower;
		this.defense = defense;
		this.magicResistance = magicResistance;

		Hp.Subscribe(val =>
		{
			HpRatio.Value = val / MaxHp.Value;
		});

		Shield.Subscribe(val =>
		{
			ShieldRatio.Value = val / MaxHp.Value;
		});
	}

	public void ResetHp()
	{
		MaxHp.Value = Extensions.IncreasePercent(MaxHp.Value,GetAddTotalExtraAttribute(AttributeType.MaxHp));
		Hp.Value = MaxHp.Value;
	}
	
	public void SetAdditionalAttribute(AttributeModifier attribute)
	{
		attributeModifier.Add(attribute);
	}
	public void RemoveAdditionalAttribute(AttributeModifier attribute)
	{
		attributeModifier.Remove(attribute);
	}
	public float GetAddTotalExtraAttribute(AttributeType type)
	{
		return attributeModifier.GetAddTotal(type);
	}
	public float GetMulTotalExtraAttribute(AttributeType type)
	{
		return attributeModifier.GetMulTotal(type);
	}
}

[System.Serializable]
public class EnemyAttribute : Attribute
{
	private float moveSpeed;
	public EnemyAttribute(float maxHp, float attackSpeed, float atkPower, float defense, float magicResistance, float moveSpeed) : base(maxHp, attackSpeed, atkPower, defense, magicResistance)
	{
		this.moveSpeed = moveSpeed;
	}

	public float MoveSpeed => moveSpeed;
}

[System.Serializable]
public class OperatorAttribute : Attribute
{
	private float reDeployTime;
	public ObservableValue<int> Cost { get; private set; }
	private int block;

	public int Block => block;
	public OperatorAttribute(float maxHp, float attackSpeed, float atkPower, float defense, float magicResistance, float reDeployTime, int cost, int block) : base(maxHp, attackSpeed, atkPower, defense, magicResistance)
	{
		this.reDeployTime = reDeployTime;
		Cost = new ObservableValue<int>(cost);
		this.block = block;
	}
	public float GetRedeployTime() => reDeployTime;
	public float GetTestRespawnTime() => 50f;
}



