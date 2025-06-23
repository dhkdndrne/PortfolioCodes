using UnityEngine;

public class Buff_Shield : Buff
{
	public Buff_Shield(float duration, bool isGroup, float totalShield) : 
		base(duration, isGroup)
	{
		TotalShield = totalShield;
		RemainingShield = totalShield;
	}
	public float TotalShield { get; private set; }
	public float RemainingShield { get; private set; }
	private Unit owner;
	
	public override void Apply(Unit target)
	{
		owner = target;
		target.AdjustShieldValue(TotalShield);
	}

	public override void Remove(Unit target)
	{
		// 남은 실드 만큼 차감하여 유닛의 실드 값을 보정
		target.AdjustShieldValue(-RemainingShield);
		owner = null;
	}

	/// <summary>
	/// 실드량에서 남은 피해량 반환
	/// </summary>
	/// <param name="damage"></param>
	/// <returns></returns>
	public float AbsorbDamage(float damage)
	{
		float absorbed = Mathf.Min(damage, RemainingShield);
		RemainingShield -= absorbed;

		if (owner != null)
		{
			owner.AdjustShieldValue(-absorbed);
		}

		if (RemainingShield < 0)
			RemainingShield = 0;

		return damage - absorbed; // 남은 피해량 반환
	}
}
