using System;

public enum ReplayEventType
{
	Deploy,		// 배치
	UseSkill,	// 스킬사용
	Retreat		// 퇴각
}
#region 타일맵

public enum GameState
{
	Init,
	Playing,
	Win,
	Lose
}

/// <summary>
/// 유닛 공격범위
/// </summary>
public enum GridType
{
	None,
	Pivot,     // 기준점
	CanAttack, // 공격가능
}

public enum WayPointType
{
	Move,
	Stay
}

public enum HeightType
{
	Lowland,
	Highland,
}
[Flags]
public enum TileType
{
	//tiles{tileKey,heightType = "LOWLAND",buildableType,passableMask,playerSideMask}
	Blocked = 1 << 0,    // 배치 불가
	Road = 1 << 1,       // 길
	HeightOnly = 1 << 2, // 공중으로만 다닐수 있음
	Deployable = 1 << 3  // 배치 가능
}

#endregion

#region 스킬관련

public enum SpChargeType
{
	PerSecond,
	Attacking
}

public enum SkillActiveType
{
	Auto,
	Manual
}

#endregion

public enum Damage_Type
{
	Physical,
	Magic,
	True
}

public enum AttributeModifierType
{
	Add,
	Multiply
}
public enum AttributeType
{
	Hp,
	MaxHp,
	Damage,
	Defense,
	MagicResistance,
	AtkSpeed,
	ReDeployTime,		//재배치 시간
	Target,
	Healing,
	Cost,
	MagicEvasion,
	MeleeEvasion,
	Length
}


#region 오퍼레이터

public enum Operator_AtkType
{
	Melee,
	Ranged
}

public enum Operator_Class
{
	Vanguard,
	Guard,
	Defender,
	Sniper,
	Caster,
	Supporter,
	Specialist,
	Medic
}

public enum Operator_State
{
	Deploy,
	Idle,
	Attack,
	Skill
}

public enum OperatorDirection
{
	None,
	Right,
	Down,
	Left,
	Up,
}

public enum OperatorID
{
	SilverAsh = 1000,
	Exusiai,
	Myrtle,
	Nightingale
}

#endregion

#region 적

public enum Enemy_State
{
	Idle,
	Move,
	Attack
}

#endregion