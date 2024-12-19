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
	Super = 10,
	Bomb = 30,
	L_Slash = 40,
	R_Slash = 50,
	Vertical = 60,
	Boomerang = 70,
	//스페셜 조합
	Munchkin = Super * Super,
	Special_Bomb = Super * Bomb,
	Special_RSlash = Super * R_Slash,
	Special_LSlash = Super * L_Slash,
	Special_Vertical = Super * Vertical,
	Special_Boomerang = Super * Boomerang,
	
	//폭탄 조합
	Bomb_Bomb = Bomb * Bomb,
	Bomb_RSlash = Bomb * R_Slash,
	Bomb_LSlash = Bomb * L_Slash,
	Bomb_Vertical = Bomb * Vertical,
	Bomb_Boomerang = Bomb * Boomerang,
	
	//슬래쉬 조합
	Slash_Slash = L_Slash * R_Slash,
	LSlash_Vertical = L_Slash * Vertical,
	RSlash_Vertical = R_Slash * Vertical,
	LSlash_Boomerang = L_Slash * Boomerang,
	RSlash_Boomerang = R_Slash * Boomerang,
	
	// 나머지 조합
	// 세로 + 세로는 그냥 세로라 패스
	Vertical_Boomerang = Vertical * Boomerang,
	Boomerang_Boomeran = Boomerang * Boomerang	
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