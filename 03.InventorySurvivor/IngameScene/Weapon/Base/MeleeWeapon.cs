using System;
using UnityEngine;
public class MeleeWeapon : Weapon
{
	protected override void Attack()
	{
		
	}
	protected override void OnTriggerEnter(Collider other)
	{
		float additionalValue = 0;
		
		if (ItemSo.Skill.SkillType == SkillType.Buff)
		{
			var val = ItemSo.Skill as BuffSkill;
			additionalValue = val.GetBuffValue(AbilityType.MeleeAtkPower);
		}
		
		other.GetComponent<Enemy>().Hit(DamageType.Melee,ItemSo.GetApplyAbilityDamage(additionalValue));
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Z))
		{
			animator.SetTrigger("Attack");
		}
	}

	private void ColEnable()
	{
		Debug.Log("콜라이더~");
	}
	private void EndAttack()
	{
		Debug.Log("엔드 어택");
	}
}
