using System;
using UnityEngine;

public static class CombatFormulaUtil
{
	/// <summary>
	/// 공격 속도 공식 적용
	/// 공격 속도 = 1 / ((100 + 공속 보너스) / (초기 공격속도 + 공격속도 변경치) / 100)
	/// </summary>
	/// /// <param name="initialAttackInterval"> 초기 공격속도</param>
	/// <param name="attackSpeedBonus"> 공속 보너스</param>
	/// <param name="attackSpeedDelta"> 공격 속도 변경치 </param>
	/// <returns></returns>
	public static float CalculateAttackInterval(float initialAttackInterval, float attackSpeedBonus, float attackSpeedDelta)
	{
		// 이론적인 공격 간격 계산
		float atkSpeed = 1f / ((1f + attackSpeedBonus) / (initialAttackInterval + attackSpeedDelta));

		// 프레임 보정: 이론 간격을 초당 프레임(30프레임 기준)으로 변환한 뒤 반올림
		float frameCorrection = MathF.Round(atkSpeed * 30f);

		// 보정된 프레임 수를 다시 초 단위로 환산
		float finalInterval = frameCorrection / 30f;

		// 최종 결과를 소수점 4자리까지 반올림
		return MathF.Round(finalInterval, 4);
	}

	/// <summary>
	/// 최종 물리데미지 계산
	/// </summary>
	/// <param name="attacker"></param>
	/// <param name="target"></param>
	/// <returns></returns>
	public static float GetFinalMeleeDamage(Unit attacker,Unit target)
	{
		// 물리 대미지={(캐릭터 최종 공격력)-(적 최종 물리 방어력)}x(취약 효과)
		float damage = attacker.GetFinalDamage();
		float defense = target.GetFinalMeleeDefense();
		float finalDamage = damage - defense;

		// 최소치 보정
		if (finalDamage <= 0)
		{
			finalDamage = damage * 0.05f;
		}
		return finalDamage;
	}


	/*
 *	물리공격 데미지 공식 : f = atk -(d-i[f]) * (1 - i[s]) * (amp * red)
 *						d = [(def + mod1) × (1 + b) − mod2] × (1 − mod3)
 *	f = 최종데미지
 *  atk = 모든 공격 버프와 배율이 적용된 공격력
 *  i[f] = 방무 고정값
 *	i[s] = 특정 스탯(공격력, 레벨 등)에 비례하여 방어력 무시량이 증가  ex) 공격력의 20%만큼 방어력 무시
 *
 *	d = 모든 버프,디버프가 적용된 방어력
 *	mod1 = 고정 def 증가의 합계
 *  mod2 = 고정 DEF 감소의 합계를 나타냅니다
 *  mod3 = 적용된 모든 스케일링 방어력 디버프를 곱한 최종 값
 *	b = 모든 스케일링 방어력 버프의 합계
 *  amp = 모든 물리 피해 증폭 효과의 합을 나타낸다.
 *  red = 모든 물리 피해 감소 효과의 합을 나타낸다.
 *
 *  f 의 최솟값은 atk의 5%
 *  def의 최소값은 0
 *
 * -------------------------------------------------------------------------------------------
 * 


	캐릭터 최종 공격력={(기초공격력)*(합연산 퍼센테이지 총합)+격려 버프}*(곱연산 퍼센테이지)

	적 최종 물리 방어력={(적 기초 물리방어력)+(방어력 버프)}*(1-방깎 퍼센테이지)*(1-방깎 퍼센테이지)*… (복리계산)
 */


	/* 최종방어력
	 * {(적 기초 물리방어력)+(방어력 버프)}*(1-방깎 퍼센테이지)*(1-방깎 퍼센테이지)*…
	 *
	 */
}