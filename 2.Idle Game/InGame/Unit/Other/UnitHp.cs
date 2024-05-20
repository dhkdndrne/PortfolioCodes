using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(UI_UnitHp))]
public class UnitHp : MonoBehaviour
{
   #region Inspector

	[SerializeField] private double maxHp;
	[SerializeField] private double hp;

    #endregion

    #region Field

	private UI_UnitHp unitHpUI;
	public Action onDeathAction;
	private bool isDead;

    #endregion

	private void Awake()
	{
		unitHpUI = GetComponent<UI_UnitHp>();
	}

	public bool IsDead => isDead;

	public void Init(double hp)
	{
		onDeathAction = null;
		SetMaxHp(hp);
		this.hp = maxHp;
		isDead = false;
		unitHpUI.Init();
	}

	public void SetMaxHp(double hp)
	{
		maxHp = hp;
	}

	public void ResetHp()
	{
		hp = maxHp;
		isDead = false;
		unitHpUI.UpdateSlider(1);
	}
	
	public void Hit(double damage,bool isCritical = false)
	{
		if (isDead)
			return;

		var textType = isCritical ? TextType.CriDamage : TextType.Damage;

		if (!LowBatterySystem.Instance.IsLowBatterMode)
		{
			var damageText = ObjectPoolManager.Instance.Spawn("DamageText");
			damageText.GetComponent<DamageText>().InitText(damage.TranslateNumber(),textType, transform.position);
		}

		hp -= damage;
		unitHpUI.UpdateSlider((float)(hp / maxHp));
        
		if (hp <= 0)
		{
			isDead = true;
			onDeathAction?.Invoke();
			hp = 0;
		}
	}
	public void Heal(double healValue)
	{
		if (isDead || hp >= maxHp)
			return;

		hp += healValue;

		if (!LowBatterySystem.Instance.IsLowBatterMode)
		{
			var damageText = ObjectPoolManager.Instance.Spawn("DamageText");
			damageText.GetComponent<DamageText>().InitText(healValue.TranslateNumber(),TextType.Heal, transform.position);
		}
        
		if (hp > maxHp)
			hp = maxHp;

		unitHpUI.UpdateSlider((float)(hp / maxHp));
	}
}