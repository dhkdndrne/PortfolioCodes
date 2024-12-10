using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OwnEffect
{
	[SerializeField] private OwnEffectType effectType;
	[SerializeField] private double effect;
	[SerializeField] private double levelUpeffect;

	private double baseEffect;
	
	public OwnEffectType EffectType => effectType;
	public double Effect
	{
		get
		{
			return effect;
		}
		set
		{
			effect = value;
		}
	}

	public OwnEffect(OwnEffectType type, double value, double levelUpEffect,int level)
	{
		effectType = type;
		effect = value * levelUpEffect * level;
		baseEffect = value;
		levelUpeffect = levelUpEffect;
	}

	public void LevelUp(int level)
	{
		effect = baseEffect * levelUpeffect * level;

		if (effectType == OwnEffectType.Health)
			Player.Instance.OnChangeHPAction?.Invoke();
	}
}

public static class OwnEffectMethod
{
	public static string GetOwnEffectTypeToString(this OwnEffectType type)
	{
		return type switch
		{
			OwnEffectType.Damage => "공격력",
			OwnEffectType.Gold => "골드 획득량",
			OwnEffectType.Health => "체력",
			OwnEffectType.CriDamage => "치명타 피해",
			OwnEffectType.MoveSpeed => "이동속도"
		};
	}
}