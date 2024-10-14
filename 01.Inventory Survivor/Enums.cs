public enum LogType
{
	Log,
	LogError,
	Warning,
	Try,
	Success
}

public enum ItemType
{
	Weapon,
	Aromor,
	Accessory
}

public enum ItemRarity
{
	Common = 0,
	UnCommon,
	Rare,
	Unique,
	Legend
}

public enum DamageType
{
	Melee,
	Magic,
	Projectile
}

public enum GameStep
{
	Playing = 0,
	UnLockSlot,
	InventoryArrange
}

public enum StringColor
{
	Red,
	Green,
	Black,
	Yellow,
	White
}

public enum SkillType
{
	Active,
	Passive,
	Buff
}
#region 능력치 / 시너지

public enum AbilityType
{
	Hp = 0, // 현재 체력
	MaxHp,      // 최대 체력

	Damage,             // 피해량
	MeleeAtkPower,      // 근접 데미지
	ProjectileAtkPower, // 투사체 데미지
	MagicAtkPower,      // 마법 데미지

	CriChance,   // 치명타 확률
	CriPower,    // 치명타 데미지
	DodgeChance, // 회피
	AtkSpeed,    // 공격 속도
	MoveSpeed,   // 이동 속도
	AtkRange,    // 공격 범위

	Vitality,  //활력
	LifeSteal, // 흡혈
	Recovery,  // 회복력
	Revival,   // 부활
	Armor,     // 방어력

	Luck,     // 행운
	GoldGain, // 추가 골드
	ExpGain,  // 추가 경험치
	Length
}

public enum SynergyKeyword
{
	Axe = 5000,
	Sword,
	SpellBook
}
#endregion