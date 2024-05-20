using System;
using System.Collections.Generic;
using Bam.Singleton;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Player : Singleton<Player>
{
    #region Field
    
	public Item_Character EquippedCharacter { get; private set; }
	
	private Item_Equipment[] equippedEquipment = new Item_Equipment[2];

	private Subject<EquipmentType> equipmentChangedSubject = new();
	public IObservable<EquipmentType> EquipmentChanged => equipmentChangedSubject;
	public Dictionary<StatType, PlayerStat> StatDic { get; private set; }                        // 업그레이드 된 스탯
	public ReactiveDictionary<OwnEffectType, double> OwnEffectDic { get; private set; } = new(); // 아이템 소유 효과 저장할 딕셔너리

	public Item_Pet[] EquippedPet { get; private set; } = new Item_Pet[5];
	
	public PlayerCurrency Currency { get; private set; }
	public SkillSystem SkillSystem { get; private set; }

	[field: SerializeField] public PlayerSprites[] PlayerSprites { get; private set; }
	
	[field: SerializeField] public UnitAI PlayerUnit { get; private set; }

	public Action OnChangeHPAction;
	public Action<int, int> onEquipPetAction;
    
    #endregion

	protected override void Awake()
	{
		if (SceneManager.GetActiveScene().name.Equals("TitleScene"))
		{
			UtilClass.DebugLog("타이틀 씬에서 플레이어 클래스 생성되어서 파괴", LogType.Warning);
			Destroy(gameObject);
			return;
		}

		base.Awake();

		Currency = new PlayerCurrency();
		StatDic = new Dictionary<StatType, PlayerStat>();
		SkillSystem = new SkillSystem();

		var initializator = Initializator.Instance;
		
		initializator.onFirstInit += InitData;
		initializator.onSecondInit += InitCharacter;
		initializator.onSecondInit += SkillSystem.Init;
		initializator.onSecondInit += () =>
		{
			var idList = DataManager.Instance.PlayerData.equippedPetIDs;
			for (int i = 0; i < idList.Length; i++)
			{
				var petData = idList[i];

				if (petData.id != 0)
				{
					var pet = ItemManager.Instance.GetPetData(petData.rank, petData.id);
					EquipPet(pet, i);
				}
			}
		};
		
        OnChangeHPAction += () =>
		{
			PlayerUnit.MyHp.SetMaxHp(GetTotalHp());
		};
	}

	private void InitData()
	{
		var enumValue = Enum.GetValues(typeof(StatType));
		var dataManager = DataManager.Instance;

		for (int i = 0; i < enumValue.Length; i++)
		{
			var stat = (StatType)i;

			var data = dataManager.GetPlayerBaseStat(stat);

			int level = dataManager.PlayerData.statUpgradeLevel[i];
			int maxLevel = int.Parse(data["maxLv"].ToString());
			float startEffect = Convert.ToSingle(data["start_effect"].ToString());
			int startCost = int.Parse(data["start_cost"].ToString());
			float effectIncrease = Convert.ToSingle(data["effect_increase"].ToString());
			float costIncrease = Convert.ToSingle(data["cost_increase"].ToString());

			bool isPlus = (StatType)i is StatType.CriRate or StatType.AtkSpeed; //합연산으로 체크해야할 공식

			StatDic.Add(stat, new PlayerStat(level, maxLevel, startEffect, effectIncrease, startCost, costIncrease, isPlus));
		}
        
		Currency.gold.Value = dataManager.PlayerData.gold;

		foreach (OwnEffectType effect in Enum.GetValues(typeof(OwnEffectType)))
		{
			OwnEffectDic.Add(effect, 0);
		}
	}

	private void InitCharacter()
	{
		var playerData = DataManager.Instance.PlayerData;

		EquippedCharacter = ItemManager.Instance.GetCharacterDic()[playerData.equippedCharacterID];

		SetEquipment(ItemManager.Instance.GetEquipmentData(playerData.equipedItems[0].rank, playerData.equipedItems[0].id));
		SetEquipment(ItemManager.Instance.GetEquipmentData(playerData.equipedItems[1].rank, playerData.equipedItems[1].id),true);
	}

	public Item_Equipment GetEquipment(EquipmentType type)
	{
		return equippedEquipment[(int)type];
	}

	public void SetEquipment(Item_Equipment newItem,bool isInit = false)
	{
		equippedEquipment[(int)newItem.Equipment_Type] = newItem;

		// 변경 사항을 Subject를 통해 알립니다.
		equipmentChangedSubject.OnNext(newItem.Equipment_Type);
		
		if(!isInit && newItem.Equipment_Type == EquipmentType.Armor)
			OnChangeHPAction?.Invoke();
	}

	public void EquipCharacter(Item_Character character)
	{
		EquippedCharacter = character;
		
		var so = ItemManager.Instance.CharaterSpriteSODic[EquippedCharacter.ID];
		PlayerSprites[0].ChangeSprite(so);
		PlayerSprites[1].ChangeSprite(so);
		
		DataManager.Instance.RefreshEquippedCharacter(character.ID);
	}
	
	public void EquipPet(Item_Pet pet,int idx)
	{
		int index = idx != -1 ? idx : Array.FindIndex(EquippedPet, slot => slot == null);
    
		if (index != -1)
		{
			EquippedPet[index] = pet;
			pet.Equip();
			(PlayerUnit.unitBase as Unit_Player).EquipPet(index,pet.AC);
			onEquipPetAction?.Invoke(pet.ID,index);
			DataManager.Instance.RefreshEquippedPet(index,pet);
		}
	}

	public void UnEquipPet(int id)
	{
		int idx = Array.FindIndex(EquippedPet, pet => pet != null && pet.ID == id);
		
		if (idx == -1)
		{
			UtilClass.DebugLog("장착된 펫 중 같은 아이디가 없음",LogType.LogError);
			return;
		}
		
		EquippedPet[idx].UnEquip();
		EquippedPet[idx] = null;
		(PlayerUnit.unitBase as Unit_Player).UnEquipPet(idx);
		DataManager.Instance.RefreshEquippedPet(idx,null);
		onEquipPetAction?.Invoke(0, idx);
	}
	
	public double GetTotalDamage()
	{
		return StatDic[StatType.Damage].effect.Value * (1 + (equippedEquipment[0].EquippedEffect +OwnEffectDic[OwnEffectType.Damage]));
	}

	public double GetTotalHp()
	{
		return StatDic[StatType.Health].effect.Value * (1 + (equippedEquipment[1].EquippedEffect + OwnEffectDic[OwnEffectType.Health]) * 0.01f);
	}
}