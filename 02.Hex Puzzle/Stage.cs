using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bam.Singleton;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class Stage : ObjectSingleton<Stage>
{
	public ReactiveProperty<int> StageNum { get; private set; } = new();
	public ReactiveProperty<int> Score { get; private set; } = new();
	public ReactiveProperty<int> MoveCnt { get; private set; } = new();

	private StageData stageData;
	public StageData StageData => stageData;
	public event Action<Sprite, int> OnTargetUpdated;
	
	public void LoadStage(StageData data)
	{
		stageData = data;
		Score.Value = 0;
		StageNum.Value = data.StageNum;
		MoveCnt.Value = stageData.MoveCnt;

		// targetDic = data.GetTargetList()
		// 	.Select(x => new KeyValuePair<Sprite, int>(x.targetData.targetToken.sprites[0], x.count))
		// 	.ToDictionary(pair => pair.Key, pair => pair.Value);
	}
	
	public void UpdateBlockTarget(Block block, int newCount)
	{
		// if (targetDic.ContainsKey(sprite))
		// {
		// 	targetDic[sprite] += newCount;
		//
		// 	if (targetDic[sprite] <= 0)
		// 		targetDic[sprite] = 0;
		// 	
		// 	OnTargetUpdated?.Invoke(sprite,targetDic[sprite]);
		// }
	}
}