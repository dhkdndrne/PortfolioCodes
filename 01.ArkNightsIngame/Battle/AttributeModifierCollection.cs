using System.Collections.Generic;
using System.Linq;

/// <summary>
/// AttributeModifierCollection은 특정 AttributeType(예: 공격력, 방어력 등)에 대해  
/// 추가/곱 연산 방식의 AttributeModifier들을 분리하여 관리하는 클래스입니다.  
/// 
/// - AddModifiers: 단순 가산형 버프 (예: +15 공격력)
/// - MultiplyModifiers: 곱 연산 기반 버프 (예: -20% 피해 감소 = * 0.8)
/// 
/// 이 클래스는 ModifierType에 따라 적절한 리스트에 버프를 추가하거나 제거하며,  
/// 최종 연산에 필요한 누적 합/곱 값을 제공하는 유틸리티 역할을 수행합니다.
/// </summary>
/// 
public class AttributeModifierCollection
{
	private List<AttributeModifier> modifiers = new();
    
	private readonly Dictionary<AttributeType, float> addDic = new();
	private readonly Dictionary<AttributeType, float> mulDic = new();
	private readonly HashSet<AttributeType> dirtyTypes = new(); // 변경이 있는 능력치를 저장하는 hashset

	public void Add(AttributeModifier mod)
	{
		modifiers.Add(mod);
		dirtyTypes.Add(mod.AttributeType);
	}

	public void Remove(AttributeModifier mod)
	{
		modifiers.Remove(mod);
		dirtyTypes.Add(mod.AttributeType);
	}
	
	public float GetAddTotal(AttributeType type)
	{
		if (dirtyTypes.Contains(type)) Recalculate(type);
		return addDic.TryGetValue(type, out var value) ? value : 0f;
	}

	public float GetMulTotal(AttributeType type)
	{
		if (dirtyTypes.Contains(type)) Recalculate(type);
		return mulDic.TryGetValue(type, out var value) ? value : 1f;
	}

	/// <summary>
	/// 능력치 재계산
	/// </summary>
	/// <param name="type"></param>
	private void Recalculate(AttributeType type)
	{
		float add = 0f;
		float mul = 1f;

		foreach (var mod in modifiers)
		{
			if (mod.AttributeType != type) continue;

			if (mod.Type == AttributeModifierType.Add)
				add += mod.Value;
			else
				mul *= (1f - mod.Value);
		}

		addDic[type] = add;
		mulDic[type] = mul;
		dirtyTypes.Remove(type);
	}
}

public readonly struct AttributeModifier
{
	public AttributeType AttributeType { get; }

	public float Value { get; }
	
	public AttributeModifierType Type { get; }
	
	// 모든 필드를 명시해야 하며 불변
	public AttributeModifier(AttributeType attributeType, float value, AttributeModifierType type)
	{
		AttributeType = attributeType;
		Value = value;
		Type = type;
	}
}
