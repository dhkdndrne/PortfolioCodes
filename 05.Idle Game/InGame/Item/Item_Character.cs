using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Item_Character : ItemBase
{
	public double LevelUpBaseCost { get; private set; }
	public double LevelPerCostRate { get; private set; }
	public double BuyCost { get; private set; }
	
	public OwnEffect[] OwnEffects { get; private set; }= new OwnEffect[6];
	public Item_Skill OriginalSkill { get; private set; }
	
	public void Init(int id,bool isLock, string itemName,ItemRankType rank, int level,
		int maxLevel, int piece, int maxPiece, OwnEffect[] ownEffects,Item_Skill skill,double buyCost,double levelPerCostRate,double levelUpBaseCost)
	{
		ID = id;
		ItemName = itemName;
		OwnEffects = ownEffects;
		Rank = rank;

		Level.Value = level;
		this.maxLevel = maxLevel;
		this.piece.Value = piece;
		MaxPiece = maxPiece;
		IsLock.Value = isLock;
		OriginalSkill = skill;
		Player player = Player.Instance;

		BuyCost = buyCost;
		LevelUpBaseCost = levelUpBaseCost;
		LevelPerCostRate = levelPerCostRate;
		
		if (!isLock)
		{
			foreach (var effect in ownEffects)
			{
				player.OwnEffectDic[effect.EffectType] += effect.Effect;
			}
		}
		else
		{
			IsLock.DistinctUntilChanged().Where(value => !value).Subscribe(_ =>
			{
				foreach (var effect in ownEffects)
				{
					player.OwnEffectDic[effect.EffectType] += effect.Effect;

					if (effect.EffectType == OwnEffectType.Health)
						player.OnChangeHPAction?.Invoke();
				}
                
				Debug.Log($"{id} 잠금 해제");
			});
		}
	}

	public override void LevelUp()
	{
		if (Level.Value >= maxLevel)
			return;
		
		Level.Value++;
		piece.Value -= MaxPiece;
	}
}
