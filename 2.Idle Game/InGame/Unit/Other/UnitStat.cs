using UnityEngine;

[System.Serializable]
public class UnitStat
{
	public double Damage { get; set; }
	public float AtkRange { get; set; }
	public float AtkSpeed { get; set; }
	public float MoveSpeed { get; set; }
	public float CriRate { get; set; }
	public double CriDamage { get; set; }
	public float AtkCoolTime { get; set; }

	public UnitStat(double damage, float atkRange, float atkSpeed, float moveSpeed, double criDamage, float criRate)
	{
		Damage = damage;
		AtkRange = atkRange;
		AtkSpeed = atkSpeed;
		MoveSpeed = moveSpeed;
		CriDamage = criDamage;
		CriRate = criRate;
		AtkCoolTime = 0;
	}
}