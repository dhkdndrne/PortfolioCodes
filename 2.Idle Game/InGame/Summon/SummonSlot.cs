using System;
using UniRx;
using Bam.Extensions;
using UnityEngine;

public class SummonSlot
{
	public ReactiveProperty<int> Level { get; private set; } = new ReactiveProperty<int>(1);
	public ReactiveProperty<int> Exp { get; private set; } = new ReactiveProperty<int>(1);
	public ReactiveProperty<int> MaxExp { get; private set; } = new ReactiveProperty<int>(1);
	
	private int summonType;
	private Action<int> summon_10_Acion;
	private Action<int> summon_100_Acion;
	
	public void Init(int level, int exp,int maxExp,int summonType, Action<int> summon_10_Acion, Action<int> summon_100_Acion)
	{
		Level.Value = level;
		Exp.Value = exp;
		MaxExp.Value = maxExp;
		
		if (Level.Value == Define.MAX_SUMMON_LEVEL)
		{
			MaxExp.Value = 1;
			Exp.Value = 1;
		}
		
		this.summonType = summonType;
		this.summon_10_Acion = summon_10_Acion;
		this.summon_100_Acion = summon_100_Acion;
	}

	public void Summon_10()
	{
		GetExp(10);
		summon_10_Acion?.Invoke(summonType);
	}

	public void Summon_100()
	{
		GetExp(100);
		summon_100_Acion?.Invoke(summonType);
	}

	private void GetExp(int value)
	{
		if (Level.Value == Define.MAX_SUMMON_LEVEL)
		{
			MaxExp.Value = 1;
			Exp.Value = 1;
			return;
		}
		
		Exp.Value += value;
		
		while (Exp.Value >= MaxExp.Value)
		{
			Level.Value++;

			Exp.Value = Extensions.Abs(MaxExp.Value - Exp.Value);
			MaxExp.Value *= 2;
		}
        
		DataManager.Instance.RefreshSummonData(summonType,Level.Value, Exp.Value, MaxExp.Value);
	}
}