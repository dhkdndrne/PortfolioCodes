using System;
using System.Collections.Generic;
using UniRx;
using Bam.Extensions;
using Cysharp.Threading.Tasks;
public class PlayerData : ObjectSingleton<PlayerData>
{
	public CharacterStatData so;
	private Dictionary<AbilityType, ReactiveProperty<int>> abilityDic = new Dictionary<AbilityType, ReactiveProperty<int>>();
	
	private int level;
	private int LevelUpCnt;
	public int MaxExp { get; private set; }
	
	public ReactiveProperty<int> Exp { get;}= new ReactiveProperty<int>();
	public ReactiveProperty<int> Gold { get;} = new ReactiveProperty<int>();

	public ReactiveProperty<int> GetAbilityProperty(AbilityType abilityType) => abilityDic[abilityType];
	public void ChangeAbilityValue(AbilityType abilityType, int value) => abilityDic[abilityType].Value += value;
	
	public PlayerData()
	{
		foreach (AbilityType e in Enum.GetValues(typeof(AbilityType)))
		{
			abilityDic.Add(e, new ReactiveProperty<int>());
		}

		level = 1;
		Exp.Value = 0;
		MaxExp = DataManager.Instance.PlayerExpList[level];
		Gold.Value = 10000;
		LevelUpCnt = 0;
		
		//임시
		abilityDic[AbilityType.Hp].Value = 100;
		abilityDic[AbilityType.MaxHp].Value = 100;
	}

	public int GetLevelUpCnt()
	{
		int temp = LevelUpCnt;
		LevelUpCnt = 0;

		return temp;
	}

	public void UpdateExp(int value)
	{
		bool isPlus = abilityDic[AbilityType.ExpGain].Value > 0;
		Exp.Value += isPlus ? (int)Extensions.IncreasePercent(value, abilityDic[AbilityType.ExpGain].Value) : (int)Extensions.DecreasePercent(value, abilityDic[AbilityType.ExpGain].Value);
			
		while (level < 43 && Exp.Value >= MaxExp)
		{
			Exp.Value = MaxExp - Exp.Value;
			MaxExp = DataManager.Instance.PlayerExpList[++level];
			LevelUpCnt ++;
		}
	}

	public void UpdateGold(int value)
	{
		Gold.Value = (int)Extensions.IncreasePercent(value, abilityDic[AbilityType.GoldGain].Value);
	}
}