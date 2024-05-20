using System;
using System.IO;
using System.Collections.Generic;
using Bam.Singleton;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using File = System.IO.File;

public class DataManager : DontDestroySingleton<DataManager>
{
	#region Data

	private Dictionary<string, Dictionary<string, object>> playerBaseStatData;
	private Dictionary<string, Dictionary<string, object>> skillData;
	private Dictionary<string, Dictionary<string, object>> petData;
	private Dictionary<string, List<(object, object)>> summonData;
	private Dictionary<string, Dictionary<string, object>> enemyStatData;
	private Dictionary<string, Dictionary<string, object>> equipmentData;
	private Dictionary<string, Dictionary<string, List<object>>> stageData;
	private Dictionary<string, Dictionary<string, List<object>>> characterData;

    #endregion

	#region Field

	private string path;
	private string dicPath;

    #endregion

	#region Property

	public PlayerSaveData PlayerData { get; private set; }
	public Dictionary<string, Dictionary<string, object>> GetSkillData { get => skillData; }
	public Dictionary<string, List<(object, object)>> GetSummonData { get => summonData; }
	public Dictionary<string, Dictionary<string, object>> GetEnemyData { get => enemyStatData; }
	public Dictionary<string, Dictionary<string, object>> GetEquipmentData { get => equipmentData; }
	public Dictionary<string, Dictionary<string, List<object>>> GetStageData { get => stageData; }
	public Dictionary<string, Dictionary<string, List<object>>> GetCharacterData { get => characterData; }

	public Dictionary<string, Dictionary<string, object>> GetPetData { get => petData; }

    #endregion

	protected override void Awake()
	{
		base.Awake();
		path = Path.Combine(Application.persistentDataPath, "savedata.json");
		dicPath = Path.Combine(Application.persistentDataPath, "dicSavedata.json");
	}

	public async UniTask Init()
	{
		var task_1 = CSVReader.Read("PlayerBaseStat");
		var task_2 = CSVReader.Read("SkillData");
		var task_3 = CSVReader.Read("Enemy_StatData");
		var task_4 = CSVReader.ReadToDicList("StageData");
		var task_5 = CSVReader.ReadToDicList("CharacterData");
		var task_6 = CSVReader.Read("EquipmentData");
		var task_7 = CSVReader.ReadToDicMultiList("SummonRateData");
		var task_8 = CSVReader.Read("PetData");


		(playerBaseStatData, skillData, enemyStatData, stageData, characterData, equipmentData, summonData,petData) =
			await UniTask.WhenAll(task_1, task_2, task_3, task_4, task_5, task_6, task_7,task_8);

		LoadData();
	}

	public Dictionary<string, object> GetPlayerBaseStat(StatType statType)
	{
		string key = statType switch
		{
			StatType.Damage => "damage",
			StatType.Health => "health",
			StatType.Heal => "heal",
			StatType.AtkSpeed => "atkspeed",
			StatType.CriRate => "crirate",
			StatType.CriDamge => "cridamage"
		};

		return playerBaseStatData[key];
	}

	#region Json 저장 로드

	public async UniTaskVoid SaveData()
	{
		Player player = Player.Instance;
		var enumValue = Enum.GetValues(typeof(StatType));

		for (int i = 0; i < enumValue.Length; i++)
		{
			PlayerData.statUpgradeLevel[i] = player.StatDic[(StatType)i].level.Value;
		}

		var equipedWeapon = Player.Instance.GetEquipment(EquipmentType.Weapon);
		var equipedArmor = Player.Instance.GetEquipment(EquipmentType.Armor);

		PlayerData.equipedItems[0].id = equipedWeapon.ID;
		PlayerData.equipedItems[0].rank = (int)equipedWeapon.Rank;

		PlayerData.equipedItems[1].id = equipedArmor.ID;
		PlayerData.equipedItems[1].rank = (int)equipedArmor.Rank;

		PlayerData.gold = player.Currency.gold.Value;

		PlayerData.isInfinityStage = StageManager.Instance.IsInfinityStage.Value;
		
		string json = JsonConvert.SerializeObject(PlayerData);
		File.WriteAllText(path, json);

		string dicJson = JsonConvert.SerializeObject(PlayerData.equipmentDatas);
		File.WriteAllText(dicPath, dicJson);

		PlayerData.time = await OffLineRewardSystem.Instance.GetWebTime();
		UtilClass.DebugLog("저장 성공", LogType.Success);
	}

	private void LoadData()
	{
		if (File.Exists(path))
		{
			string json = File.ReadAllText(path);
			PlayerData = JsonConvert.DeserializeObject<PlayerSaveData>(json);
            
			string dicJson = File.ReadAllText(dicPath);
			PlayerData.equipmentDatas = JsonConvert.DeserializeObject<Dictionary<int, SaveData_Item>>(dicJson);

			foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
			{
				CheckNewItem(type);
			}
		}
		else
		{
			PlayerData = new PlayerSaveData();

			//스탯 업그레이드 상황
			PlayerData.statUpgradeLevel = new int[Enum.GetValues(typeof(StatType)).Length];
			Array.Fill(PlayerData.statUpgradeLevel, 1);

			//골드
			PlayerData.gold = 1000000;

			foreach (ItemType type  in Enum.GetValues(typeof(ItemType)))
			{
				AddItemData(type);
			}

			//소환 슬롯 데이터
			for (int i = 0; i < Enum.GetValues(typeof(ItemType)).Length; i++)
			{
				PlayerData.summonDatas.Add(new SaveData_Summon());
			}

			string json = JsonConvert.SerializeObject(PlayerData);
			File.WriteAllText(path, json);

			string dicJson = JsonConvert.SerializeObject(PlayerData.equipmentDatas);
			File.WriteAllText(dicPath, dicJson);
		}
	}

    #endregion

	#region 데이터 갱신

	public void RefreshSummonData(int summonType, int level, int exp, int maxExp)
	{
		PlayerData.summonDatas[summonType].level = level;
		PlayerData.summonDatas[summonType].exp = exp;
		PlayerData.summonDatas[summonType].maxExp = maxExp;
	}

	public void RefreshItemData(ItemType type, ItemBase item)
	{
		Dictionary<int, SaveData_Item> targetDic = type switch
		{
			ItemType.Equipment=>PlayerData.equipmentDatas,
			ItemType.Skill =>PlayerData.skillDatas,
			ItemType.Pet=>PlayerData.petDatas,
			ItemType.Character => PlayerData.charaterDatas,
			_=> null
		};

		if (targetDic == null)
		{
			UtilClass.DebugLog("타입오류", LogType.LogError);
			return;
		}

		int id = item.ID;

		targetDic[id].level = item.Level.Value;
		targetDic[id].curPiece = item.piece.Value;
		targetDic[id].isLock = item.IsLock.Value;
	}

	public void RefreshEquippedSkill(int index, Item_Skill skill)
	{
		PlayerData.equippedSkillIDs[index].id = skill == null ? 0 : skill.ID;
		PlayerData.equippedSkillIDs[index].rank = skill == null ? 0 : (int)skill.Rank;
		SaveData();
	}

	public void RefreshEquippedPet(int index, Item_Pet pet)
	{
		PlayerData.equippedPetIDs[index].id = pet == null ? 0 : pet.ID;
		PlayerData.equippedPetIDs[index].rank = pet == null ? 0 : (int)pet.Rank;
		SaveData();
	}

	public void RefreshEquippedCharacter(int id)
	{
		PlayerData.equippedCharacterID = id;
		SaveData();
	}
	
    #endregion

	/// <summary>
	/// 새로운 아이템 추가되었는지 확인
	/// </summary>
	/// <param name="itemType"></param>
	private void CheckNewItem(ItemType itemType)
	{
		var saveData = GetPlayerSaveData(itemType);
		
		if (itemType == ItemType.Character)
		{
			var dic = characterData;
			List<string> list = new List<string>(dic.Keys);
			
			foreach (var itemID in list)
			{
				if (!saveData.ContainsKey(int.Parse(itemID)))
				{
					UtilClass.DebugLog($"신규 아이템 데이터 세이브파일에 추가 \n아이템 타입 :{itemType}");
					SaveData_Item data = new SaveData_Item();
					data.id = int.Parse(itemID);
					data.level = 1;
					data.curPiece = 0;
					data.isLock = true;

					saveData.Add(data.id, data);
				}
			}
		}
		else
		{
			var dic = GetItemDic(itemType);
			List<string> list = new List<string>(dic.Keys);
			
			//새로운 아이템 추가
			foreach (var itemID in list)
			{
				if (!saveData.ContainsKey(int.Parse(itemID)))
				{
					UtilClass.DebugLog($"신규 아이템 데이터 세이브파일에 추가 \n아이템 타입 :{itemType}");
					SaveData_Item data = new SaveData_Item();
					data.id = int.Parse(itemID);
					data.level = 1;
					data.curPiece = 0;
					data.isLock = true;

					saveData.Add(data.id, data);
				}
			}
		}
		
	}
    
	private void AddItemData(ItemType itemType)
	{
		var saveData = GetPlayerSaveData(itemType);
		List<string> list = itemType == ItemType.Character ? new List<string>(characterData.Keys) : new List<string>(GetItemDic(itemType).Keys);
		

		for (int i = 0; i < list.Count; i++)
		{
			SaveData_Item data = new SaveData_Item();

			data.id = int.Parse(list[i]);
			data.level = 1;
			data.curPiece = 0;

			if (itemType == ItemType.Equipment)
			{
				data.isLock = (data.id == PlayerData.equipedItems[0].id || data.id == PlayerData.equipedItems[1].id) ? false : true;
			}
			else if (itemType == ItemType.Character)
			{
				data.isLock = data.id == PlayerData.equippedCharacterID ? false : true;
			}
			else data.isLock = true;
			
			saveData.Add(data.id, data);
		}
	}
	
	private Dictionary<string, Dictionary<string, object>> GetItemDic(ItemType itemType)
	{
		return itemType switch
		{
			ItemType.Equipment => equipmentData,
			ItemType.Skill=> skillData,
			ItemType.Pet => petData,
		};
	}

	private Dictionary<int, SaveData_Item> GetPlayerSaveData(ItemType itemType)
	{
		return itemType switch
		{
			ItemType.Equipment => PlayerData.equipmentDatas,
			ItemType.Skill=> PlayerData.skillDatas,
			ItemType.Pet => PlayerData.petDatas,
			ItemType.Character =>PlayerData.charaterDatas
		};
	}

}