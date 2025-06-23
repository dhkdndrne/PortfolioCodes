using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bam.Singleton;
using UnityEngine;
using Random = UnityEngine.Random;


public class ItemManager : Singleton<ItemManager>
{
	private Dictionary<int, Item_Character> characterDic;
	private Dictionary<int, Dictionary<int, Item_Skill>> itemSkillDic;

	private Dictionary<int, Dictionary<int, Item_Pet>> itemPetDic;
	//장비 아이템 리스트
	private Dictionary<int, Dictionary<int, Item_Equipment>> itemEquipmentDic;

	[SerializeField] private CharacterSpriteSO[] characterSpriteSOs;
	public Dictionary<int, CharacterSpriteSO> CharaterSpriteSODic { get; private set; }
	
	//펫 아이템 리스트
	#region 뽑기

	private List<int> keyList = new();

    #endregion
	private void Awake()
	{
		Initializator.Instance.onFirstInit += Init;
	}

	private void Init()
	{
		InitSkill();
		InitCharacter();
		InitEquipment();
		InitPet();

		CharaterSpriteSODic = new();

		foreach (var so in characterSpriteSOs)
		{
			int key = int.Parse(so.name.Split("_")[0]);
			CharaterSpriteSODic.Add(key,so);
		}
	}

	/// <summary>
	/// 캐릭터 초기화
	/// </summary>
	private void InitCharacter()
	{
		characterDic = new();
		var data = DataManager.Instance.GetCharacterData;
		var playerData = DataManager.Instance.PlayerData;
		
		foreach (var v in data)
		{
			var character = new Item_Character();

			int id = int.Parse(v.Key);
			string charName = v.Value["name"][0].ToString();
			var rank = (ItemRankType)Enum.Parse(typeof(ItemRankType), v.Value["rank"][0].ToString());

			List<OwnEffect> effectList = new(6);
			var effectTypeList = v.Value["effectTypes"];
			var effectDataList = v.Value["baseEffect"];
			var levelUpEffectDataList = v.Value["levelUp_effect"];

			for (int i = 0; i < 6; i++)
			{
				var effectType = (OwnEffectType)Enum.Parse(typeof(OwnEffectType), effectTypeList[i].ToString());
				var baseEffect = NumberTranslater.TranslateStringToDouble(effectDataList[i].ToString());
				var levelUpEffect = NumberTranslater.TranslateStringToDouble(levelUpEffectDataList[i].ToString());

				var ownEffect = new OwnEffect(effectType, baseEffect, levelUpEffect, 1);

				effectList.Add(ownEffect);
			}

			int skillIndex = int.Parse(v.Value["skillIndex"][0].ToString());
			Item_Skill skill = itemSkillDic.Values
				.SelectMany(innerDic => innerDic.Values)
				.FirstOrDefault(skill => skill.ID == skillIndex);

			if (skill == null)
			{
				UtilClass.DebugLog($"스킬 데이터 오류 {skillIndex}", LogType.LogError);
				return;
			}
            
			//나중에 플레이어 데이터 불러와야 할것들
			int level = playerData.charaterDatas[id].level;
			int piece = playerData.charaterDatas[id].curPiece;
			int maxPiece = 10;
			bool isLock = playerData.charaterDatas[id].isLock;
			int maxLevel = int.Parse(data[v.Key]["maxlevel"][0].ToString());

			double buyCost = NumberTranslater.TranslateStringToDouble(data[v.Key]["buyCost"][0].ToString());
			double levelUpBaseCost = NumberTranslater.TranslateStringToDouble(data[v.Key]["levelUpBaseCost"][0].ToString());
			double LevelPerCostRate = NumberTranslater.TranslateStringToDouble(data[v.Key]["LevelPerCostRate"][0].ToString());
			
			character.Init(id,isLock,charName,rank,level,maxLevel,piece,maxPiece,effectList.ToArray(),skill,buyCost,LevelPerCostRate,levelUpBaseCost);
			characterDic.Add(id, character);
		}
	}

	/// <summary>
	/// 스킬 초기화
	/// </summary>
	private void InitSkill()
	{
		itemSkillDic = new();

		var data = DataManager.Instance.GetSkillData;
		Assembly assembly = Assembly.GetExecutingAssembly();

		var list = data.Keys.ToArray();
		var playerData = DataManager.Instance.PlayerData;
		for (int i = 0; i < list.Length; i++)
		{
			string skillName = "Skill_" + list[i];
			Type t = assembly.GetType(skillName);

			if (t == null)
			{
				//UtilClass.DebugLog($"스킬 인스턴스화 에러 \n 에러 스킬 id {id}", Define.LogType.LogError);
				continue;
			}

			var skillData = data[list[i]];

			var skill = Activator.CreateInstance(t) as SkillBase;
			skill.Init(skillData);

			int maxLevel = int.Parse(skillData["maxlevel"].ToString());
			string itemName = skillData["name"].ToString();
			int id = int.Parse(list[i]);

			var playerSkillData = playerData.skillDatas[id];
			//나중에 플레이어 데이터 불러와야 할것들
			int level = playerSkillData.level;
			int piece = playerSkillData.curPiece;
			int maxPiece = 10;
			bool isLock = playerSkillData.isLock;

			ItemRankType rank = (ItemRankType)int.Parse(skillData["rank"].ToString());

			var item = new Item_Skill();

			var effectType = (OwnEffectType)Enum.Parse(typeof(OwnEffectType), skillData["own_effectType"].ToString());
			var baseEffect = NumberTranslater.TranslateStringToDouble(skillData["own_effect_value"].ToString());

			var levelUpEffect = NumberTranslater.TranslateStringToDouble(skillData["own_effect_up"].ToString());

			OwnEffect ownEffect = new OwnEffect(effectType, baseEffect, levelUpEffect, level);
			string description = skillData["description"].ToString();

			item.Init(id, isLock, itemName, rank, level, maxLevel, piece, maxPiece, ownEffect, description, skill);

			int rankToInt = (int)rank;

			if (!itemSkillDic.ContainsKey(rankToInt))
				itemSkillDic.Add(rankToInt, new Dictionary<int, Item_Skill>());

			itemSkillDic[rankToInt].Add(id, item);

			DataManager.Instance.PlayerData.skillDatas[id].rank = rankToInt;
		}
	}

	private void InitPet()
	{
		itemPetDic = new();

		foreach (var rank in Enum.GetValues(typeof(ItemRankType)))
		{
			itemPetDic.Add((int)rank, new Dictionary<int, Item_Pet>());
		}

		var csvData = DataManager.Instance.GetPetData;
		var playerData = DataManager.Instance.PlayerData;
		foreach (var v in csvData)
		{
			Item_Pet item = new Item_Pet();

			int id = int.Parse(v.Key);
			string itemName = v.Value["name"].ToString();

			var petData = playerData.petDatas[id];
			
			int level = petData.level;
			int piece = petData.curPiece;
			int maxPiece = 10;
			bool isLock = petData.isLock;

			int maxLevel = int.Parse(csvData[v.Key]["maxlevel"].ToString());

			var effectType = (OwnEffectType)Enum.Parse(typeof(OwnEffectType), csvData[v.Key]["own_effectType"].ToString());
			var baseEffect = NumberTranslater.TranslateStringToDouble(csvData[v.Key]["own_effect_value"].ToString());
			var levelUpEffect = NumberTranslater.TranslateStringToDouble(csvData[v.Key]["own_effect_up"].ToString());

			OwnEffect ownEffect = new OwnEffect(effectType, baseEffect, levelUpEffect, level);
			var damage = NumberTranslater.TranslateStringToDouble(csvData[v.Key]["damage"].ToString());
			var coolTime = Convert.ToSingle(csvData[v.Key]["attack_speed"].ToString());

			ItemRankType rank = (ItemRankType)int.Parse(csvData[v.Key]["rank"].ToString());

			item.Init(id, isLock, itemName, rank, level, maxLevel, piece, maxPiece, ownEffect, string.Empty, damage, coolTime);
			itemPetDic[(int)rank].Add(id, item);
		}
	}

	/// <summary>
	/// 장비 초기화
	/// </summary>
	private void InitEquipment()
	{
		itemEquipmentDic = new();

		foreach (var rank in Enum.GetValues(typeof(ItemRankType)))
		{
			itemEquipmentDic.Add((int)rank, new Dictionary<int, Item_Equipment>());
		}

		var csvData = DataManager.Instance.GetEquipmentData;
		var saveData = DataManager.Instance.PlayerData.equipmentDatas;

		foreach (var v in csvData)
		{
			Item_Equipment item = new Item_Equipment();

			int id = int.Parse(v.Key);
			string itemName = v.Value["name"].ToString();

			int level = saveData[id].level;
			int piece = saveData[id].curPiece;
			int maxPiece = 10;
			bool isLock = saveData[id].isLock;

			int maxLevel = int.Parse(csvData[v.Key]["maxlevel"].ToString());

			var effectType = (OwnEffectType)Enum.Parse(typeof(OwnEffectType), csvData[v.Key]["own_effectType"].ToString());
			var baseEffect = NumberTranslater.TranslateStringToDouble(csvData[v.Key]["own_effect_value"].ToString());
			var levelUpEffect = NumberTranslater.TranslateStringToDouble(csvData[v.Key]["own_effect_up"].ToString());

			OwnEffect ownEffect = new OwnEffect(effectType, baseEffect, levelUpEffect, level);
			var equippiedEffect = NumberTranslater.TranslateStringToDouble(csvData[v.Key]["equip_effect_value"].ToString());

			ItemRankType rank = (ItemRankType)int.Parse(csvData[v.Key]["rank"].ToString());

			EquipmentType equipmentType = id / 10000 == 2 ? EquipmentType.Weapon : EquipmentType.Armor;
			item.Init(id, isLock, itemName, equipmentType, rank, level, maxLevel, piece, maxPiece, ownEffect, equippiedEffect, string.Empty);

			itemEquipmentDic[(int)rank].Add(id, item);
		}
	}
	public Dictionary<int,Item_Character> GetCharacterDic() => characterDic;
    
	public List<(int ID, int Rank)> GetItemIDList(ItemType itemType, EquipmentType equipmentType = EquipmentType.None)
	{
		return itemType switch
		{
			ItemType.Equipment => itemEquipmentDic.Values
				.SelectMany(innerDic => innerDic.Values)
				.Where(equipment => equipment.Equipment_Type == equipmentType)
				.Select(equipment => (equipment.ID, (int)equipment.Rank))
				.ToList(),

			ItemType.Skill => itemSkillDic.Values
				.SelectMany(innerDic => innerDic.Values)
				.Select(equipment => (equipment.ID, (int)equipment.Rank))
				.ToList(),
			ItemType.Pet => itemPetDic.Values
				.SelectMany(innerDic => innerDic.Values)
				.Select(equipment => (equipment.ID, (int)equipment.Rank))
				.ToList()
		};
	}
    
	public Item_Equipment GetEquipmentData(int rank, int id)
	{
		return itemEquipmentDic[rank][id];
	}
	public Item_Skill GetSkillData(int rank, int id)
	{
		return itemSkillDic[rank][id];
	}	
	public Item_Pet GetPetData(int rank, int id)
	{
		return itemPetDic[rank][id];
	}

	public Item_Character GetCharacterData(int id)
	{
		return characterDic[id];
	}
	
	public ItemBase GetRandomItem(ItemType itemType, int rank)
	{
		ItemBase item = null;
		keyList.Clear();

		switch (itemType)
		{
			case ItemType.Equipment:
				keyList.AddRange(itemEquipmentDic[rank].Keys);
				break;
			case ItemType.Skill:
				keyList.AddRange(itemSkillDic[rank].Keys);
				break;
			case ItemType.Pet:
				keyList.AddRange(itemPetDic[rank].Keys);
				break;
		}

		if (keyList.Count > 0)
		{
			int randomIndex = Random.Range(0, keyList.Count);
			int selectedKey = keyList[randomIndex];

			switch (itemType)
			{
				case ItemType.Equipment:
					item = itemEquipmentDic[rank][selectedKey];
					break;
				case ItemType.Skill:
					item = itemSkillDic[rank][selectedKey];
					break;
				case ItemType.Pet:
					item = itemPetDic[rank][selectedKey];
					break;
			}
		}

		return item;
	}

	public void RefreshItems(ItemType itemType, Dictionary<int, (int rank, int amount)> pickedItemDic)
	{
		Dictionary<int, Dictionary<int, ItemBase>> itemDic;

		switch (itemType)
		{
			case ItemType.Equipment:
				itemDic = itemEquipmentDic.ToDictionary(
					kvp => kvp.Key,
					kvp => kvp.Value.ToDictionary(innerKvp => innerKvp.Key, innerKvp => (ItemBase)innerKvp.Value)
				);
				break;

			case ItemType.Skill:
				itemDic = itemSkillDic.ToDictionary(
					kvp => kvp.Key,
					kvp => kvp.Value.ToDictionary(innerKvp => innerKvp.Key, innerKvp => (ItemBase)innerKvp.Value)
				);
				break;

			case ItemType.Pet:
				itemDic = itemPetDic.ToDictionary(
					kvp => kvp.Key,
					kvp => kvp.Value.ToDictionary(innerKvp => innerKvp.Key, innerKvp => (ItemBase)innerKvp.Value)
				);
				break;

			default:
				return;
		}

		foreach (var item in pickedItemDic)
		{
			var val = itemDic[item.Value.rank][item.Key];
			val.IsLock.Value = false;
			val.piece.Value += item.Value.amount;

			DataManager.Instance.RefreshItemData(itemType, val);
		}
	}
}