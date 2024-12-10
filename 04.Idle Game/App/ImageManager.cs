using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ImageManager : Bam.Singleton.Singleton<ImageManager>
{
	[SerializeField] private Sprite[] iconArr;
	private Dictionary<StatType, Sprite> baseStatSpriteDic;

	[SerializeField] private Sprite[] ownEffectTypeArr;
	private Dictionary<OwnEffectType, Sprite> ownEffectSpriteDic;

	[SerializeField] private Sprite[] skillIcons;
	private Dictionary<int, Sprite> skillIconDic;
	
	[SerializeField] private Sprite[] equipmentIcons;
	private Dictionary<int, Sprite> equipmentIconDic;

	[SerializeField] private Sprite[] petIcons;
	private Dictionary<int, Sprite> petIconDic;
	
	[SerializeField] private Sprite[] itemRankBGIcons;
	private Dictionary<ItemRankType, Sprite> itemRankBGDic;

	[SerializeField] private Sprite[] itemCharacterIcons;
	private Dictionary<int, Sprite> charIconDic;
	
	[field: SerializeField]
	public Material GrayScaleMaterial { get; private set; }
    
	[field:SerializeField]
	public Sprite EquippedSkillSlotBaseSprite { get; private set; }
	protected override void Awake()
	{
		base.Awake();
		Init();
	}

	private void Init()
	{
		baseStatSpriteDic = new();
		ownEffectSpriteDic = new();
		skillIconDic = new();
		equipmentIconDic = new();
		itemRankBGDic = new();
		petIconDic = new();
		charIconDic = new();
		
		foreach (Sprite sprite in iconArr)
		{
			string iconName = sprite.name.Replace("Icon_", "");

			if (Enum.TryParse(iconName, out StatType statType))
			{
				baseStatSpriteDic.Add(statType, sprite);
			}
		}
		
		foreach (Sprite sprite in ownEffectTypeArr)
		{
			string iconName = sprite.name.Replace("OwnEffect_", "");

			if (Enum.TryParse(iconName, out OwnEffectType statType))
			{
				ownEffectSpriteDic.Add(statType, sprite);
			}
		}
		
		foreach (Sprite sprite in skillIcons)
		{
			int id = int.Parse(sprite.name.Replace("Skill_", ""));
			skillIconDic.Add(id, sprite);
		}

		foreach (var sprite in petIcons)
		{
			int id = int.Parse(sprite.name.Replace("Pet_", ""));
			petIconDic.Add(id, sprite);
		}
		
		foreach (Sprite sprite in equipmentIcons)
		{
			int id = int.Parse(sprite.name.Replace("Icon_", ""));
			equipmentIconDic.Add(id, sprite);
		}

		foreach (var sprite in itemRankBGIcons)
		{
			ItemRankType rank = (ItemRankType)Enum.Parse(typeof(ItemRankType),sprite.name.Replace("ItemRank_", ""));
			itemRankBGDic.Add(rank,sprite);
		}

		foreach (var sprite in itemCharacterIcons)
		{
			int id = int.Parse(sprite.name.Replace("Icon_", ""));
			charIconDic.Add(id,sprite);
		}
	}

	public Sprite GetCharacterIcon(int id)
	{
		return charIconDic[id];
	}
	public Sprite GetBaseStatIcon(StatType statType)
	{
		return baseStatSpriteDic[statType];
	}

	public Sprite GetOwnEffectTypeIcon(OwnEffectType ownEffectType)
	{
		return ownEffectSpriteDic[ownEffectType];
	}

	public Sprite GetItemIcon(ItemType itemType, int id)
	{
		var dictionary = itemType switch
		{
			ItemType.Equipment => equipmentIconDic,
			ItemType.Skill => skillIconDic,
			ItemType.Pet => petIconDic,
			ItemType.Character => charIconDic,
			_=> null
		};

		if (dictionary == null)
		{
			UtilClass.DebugLog("아이템 타입 오류",LogType.LogError);
			return null;
		}
		
		if (dictionary.TryGetValue(id, out var sprite))
			return sprite;
		
		UtilClass.DebugLog("ID Error",LogType.LogError);
		return null;
	}
	
	public Sprite GetItemRankBg(ItemRankType rank)
	{
		return itemRankBGDic[rank];
	}
}