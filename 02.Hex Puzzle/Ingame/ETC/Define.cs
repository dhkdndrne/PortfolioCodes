using System;
using UnityEngine;

public enum HexWay
{
	Up = 0,
	LeftUp = 1,
	LeftDown = 2,
	Down = 3,
	RightDown = 4,
	RightUp = 5,
	Length
}

public enum CellType
{
	None,
	Basic,
	Spawn
}

public enum SpecialBlockType
{
	None,
	Super,
	Bomb,
	L_Slash,
	R_Slash,
	Vertical,
	
	//스페셜 조합
	Munchkin,
	Special_Bomb,
	Special_RSlash,
	Special_LSlash,
	Special_Vertical,
	
	//폭탄 조합
	Bomb_Bomb,
	Bomb_RSlash,
	Bomb_LSlash,
	Bomb_Vertical,
	
	//슬래쉬 조합
	Slash_Slash,
	LSlash_Vertical,
	RSlash_Vertical,
}


[Serializable]
public enum ColorLayer
{
	None = 0,
	Red,
	Orange,
	Yellow,
	Green,
	Blue,
	Navy,
	Purple
}

public enum CollectingTypes
{
	Destroy,
	ReachBottom,
	Spread,
	Clear
}
public enum TargetObjectType
{
	None,
	Block,
	Cell
}