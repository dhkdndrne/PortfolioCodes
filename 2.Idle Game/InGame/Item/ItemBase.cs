using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ItemBase
{
	public int ID { get; protected set; }
	public string ItemName { get; protected set; }

	public ItemRankType Rank { get; protected set; }

	public OwnEffect OwnEffect { get; protected set; }

	public ReactiveProperty<int> Level = new ReactiveProperty<int>();
	protected int maxLevel;
	public ReactiveProperty<int> piece = new ReactiveProperty<int>();
	public int MaxPiece { get; protected set; }
	protected string description;
	public virtual string GetDescription() => description;

	public ReactiveProperty<bool> IsLock = new ReactiveProperty<bool>();
	
	public void Init(int id, bool isLock, string itemName, ItemRankType rank, int level, int maxLevel, int piece, int maxPiece, OwnEffect ownEffect, string description)
	{
		ID = id;
		ItemName = itemName;
		OwnEffect = ownEffect;
		Rank = rank;

		Level.Value = level;
		this.maxLevel = maxLevel;
		this.piece.Value = piece;
		MaxPiece = maxPiece;
		IsLock.Value = isLock;
		this.description = description;

		Player player = Player.Instance;

		if (!isLock)
		{
			player.OwnEffectDic[ownEffect.EffectType] += ownEffect.Effect;
		}
		else
		{
			//IDisposable disposable = null;
			IsLock.DistinctUntilChanged().Where(value => !value).Subscribe(_ =>
			{
				player.OwnEffectDic[ownEffect.EffectType] += ownEffect.Effect;

				if (ownEffect.EffectType == OwnEffectType.Health)
					player.OnChangeHPAction?.Invoke();

				Debug.Log($"{id} 잠금 해제");
				//disposable?.Dispose();
			});
		}
	}

	public virtual void LevelUp()
	{
		if (Level.Value >= maxLevel)
			return;

		OwnEffect.LevelUp(Level.Value + 1);
		Level.Value++;
		piece.Value -= MaxPiece;
	}
}

public static class ItemRankMethod
{
	public static string GetRankTypeToString(this ItemRankType rankType)
	{
		return rankType switch
		{
			ItemRankType.Common => "<color=#705A41>노멀</color>",
			ItemRankType.UnCommon => "<color=#113EA0>희귀</color>",
			ItemRankType.Rare => "<color=#30BF3C>레어</color>",
			ItemRankType.Epic => "<color=#BE3084>에픽</color>",
			ItemRankType.Legend => "<color=#C61F34>전설</color>",
			ItemRankType.Myth => "<color=#DB7C23>신화</color>"
		};
	}
}