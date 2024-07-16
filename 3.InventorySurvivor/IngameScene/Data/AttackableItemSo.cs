using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "SO/Item/AttackableItem", fileName = "New Attack So")]
public class AttackItemSo : ItemSo
{
	[FormerlySerializedAs("weaponDamageType")]
	[Header("무기 정보")]
	[SerializeField] private DamageType damageType;
	[SerializeField] private float applyAbilityValue; //능력치 적용률
	[SerializeField] private float damage;
	[SerializeField] private float criChance;
	[SerializeField] private float criPower;
	[SerializeField] private float range;
	[SerializeField] private float atkSpeed;

	#region 아이템의 원래 능력값

	public float ApplyAbilityValue => applyAbilityValue;
	public float Damage => damage;

	public float CriChance => criChance;
	public float CriPower => criPower;
	public float Range => range;
	public float AtkSpeed => atkSpeed;

	public DamageType DamageType => damageType;
	public List<int> synergyIdList { get; private set; }
	//public List<int> synergyIdList { get;  set; }

    #endregion

	#region 능력치 적용 값

	/// <summary>
	/// 
	/// </summary>
	/// <param name="additionalValue">추가 스탯</param>
	/// <param name="isApplyAbilityValue">스탯 적용율</param>
	/// <returns></returns>
	public float GetApplyAbilityDamage(float additionalValue, bool isApplyAbilityValue = false)
	{
		var val = damage + additionalValue + damageType switch
		{
			DamageType.Melee => PlayerData.Instance.GetAbilityProperty(AbilityType.MeleeAtkPower).Value,
			DamageType.Magic => PlayerData.Instance.GetAbilityProperty(AbilityType.MagicAtkPower).Value,
			DamageType.Projectile => PlayerData.Instance.GetAbilityProperty(AbilityType.ProjectileAtkPower).Value
		};

		return isApplyAbilityValue ? val * applyAbilityValue : val;
	}

	public float GetApplyAbilityCriChance(float additionalValue) => criChance + additionalValue + PlayerData.Instance.GetAbilityProperty(AbilityType.CriChance).Value;
	public float GetApplyAbilityCriPower(float additionalValue) => criPower + additionalValue + PlayerData.Instance.GetAbilityProperty(AbilityType.CriPower).Value;

	public float GetApplyAbilityRange(float additionalValue) => range + additionalValue + PlayerData.Instance.GetAbilityProperty(AbilityType.AtkRange).Value;

	public float GetApplyAbilityAtkSpeed(float additionalValue) => atkSpeed + additionalValue + PlayerData.Instance.GetAbilityProperty(AbilityType.AtkSpeed).Value;

    #endregion

	public void SetData(string name, ItemRarity rarity, DamageType damageType, int price, float applyValue, float damage, float criChance, float criPower, float range, float atkSpeed, int[] synergyArr)
	{
		itemName = name;
		this.rarity = rarity;
		this.damageType = damageType;
		this.price = price;
		applyAbilityValue = applyValue;
		this.damage = damage;
		this.criChance = criChance;
		this.criPower = criPower;
		this.range = range;
		this.atkSpeed = atkSpeed;

		synergyIdList = synergyArr.ToList();
	}
}