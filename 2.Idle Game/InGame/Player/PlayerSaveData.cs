using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine.Serialization;

[System.Serializable]
public class PlayerSaveData
{
	public SaveData_Stage stageData = new();

	public Dictionary<int, SaveData_Item> charaterDatas = new();
	public Dictionary<int, SaveData_Item> skillDatas = new();
	public Dictionary<int, SaveData_Item> equipmentDatas = new();
	public Dictionary<int, SaveData_Item> petDatas = new();

	public SaveData_Item[] equippedSkillIDs = 
	{
		new SaveData_Item { rank = 0, id = 0 },
		new SaveData_Item { rank = 0, id = 0 },
		new SaveData_Item { rank = 0, id = 0 },
		new SaveData_Item { rank = 0, id = 0 },
		new SaveData_Item { rank = 0, id = 0 },
		new SaveData_Item { rank = 0, id = 0 }
	};
	public SaveData_Item[] equippedPetIDs = 
	{
		new SaveData_Item { rank = 0, id = 0 },
		new SaveData_Item { rank = 0, id = 0 },
		new SaveData_Item { rank = 0, id = 0 },
		new SaveData_Item { rank = 0, id = 0 },
		new SaveData_Item { rank = 0, id = 0 }
	};
	public int equippedCharacterID = 10000;

	public SaveData_Item[] equipedItems =
	{
		new SaveData_Item { rank = 0, id = 20000 },
		new SaveData_Item { rank = 0, id = 30000 },
	};

	public bool isAutoSkill;
	public bool isInfinityStage;
	public int[] statUpgradeLevel;
	public BigInteger gold;

	public string time = "";
	public List<SaveData_Summon> summonDatas = new();
}

[Serializable]
public class SaveData_Stage
{
	public int stageLevel = 1;
	public int stageWave = 0;
}

[Serializable]
public class SaveData_Item
{
	public int id;
	public int level;
	public int rank;
	public int curPiece;
	public bool isLock;
}

[Serializable]
public class SaveData_Summon
{
	public int level = 1;
	public int exp = 0;
	public int maxExp = 10;
}