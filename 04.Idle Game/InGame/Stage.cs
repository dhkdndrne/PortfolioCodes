using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bam.Extensions;
using UniRx;
using UnityEngine;
public class Stage
{
	public ReactiveProperty<int> Wave { get;} = new ReactiveProperty<int>();
	public ReactiveProperty<string> StageName { get; private set; } = new();
	public Dictionary<int, List<string>> EnemyListDic { get; private set; } = new();

	private double min, max;
	
	public void Init()
	{
		var playerStageData = DataManager.Instance.PlayerData.stageData;
		var stageData = DataManager.Instance.GetStageData;
		Wave.Value = playerStageData.stageWave;
		StageName.Value = stageData[playerStageData.stageLevel.ToString()]["name"][0].ToString();

		for (int i = 0; i < 5; i++)
		{
			string key = "wave_" + (i + 1);
			var data = stageData[playerStageData.stageLevel.ToString()][key];
			EnemyListDic[i] = data.Select(obj => obj.ToString()).ToList();
		}

		var rewardRange = stageData[playerStageData.stageLevel.ToString()]["reward"][0].ToString().Split('-');

		min = NumberTranslater.TranslateStringToDouble(rewardRange[0]);
		max = NumberTranslater.TranslateStringToDouble(rewardRange[1]);
	}

	public bool CheckGoNextStage()
	{
		bool check = false;
		
		var stageWave = ++DataManager.Instance.PlayerData.stageData.stageWave;

		if (stageWave >= 5)
		{
			stageWave = 0;
			DataManager.Instance.PlayerData.stageData.stageWave = 0;
			check = true;
		}
		
		Wave.Value = stageWave;

		return check;
	}

	public double GetReward()
	{
		return Extensions.Random(min,max);
	}
}