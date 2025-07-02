using System;
using System.Collections.Generic;
using System.Linq;
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
	
	private Dictionary<TargetToken,int> targetDic = new Dictionary<TargetToken,int>();

	public bool isClear;
	
	public void LoadStage(StageData data)
	{
		stageData = data;
		Score.Value = 0;
		StageNum.Value = data.StageNum;
		MoveCnt.Value = stageData.MoveCnt;

		targetDic.Clear(); // 기존 데이터를 초기화
		foreach (var token in stageData.GetTargetList())
		{
			if (!targetDic.ContainsKey(token)) // 중복 방지
			{
				targetDic[token] = token.count; // 초기 값을 0으로 설정하거나 원하는 값을 할당
			}
		}
	}

	public void UpdateBlockTarget(Block block, int newCount)
	{
		foreach (var t in StageData.GetTargetList())
		{
			if(t.targetData.TargetObjectType != TargetObjectType.Block)
				continue;
			
			if (((Target_Block_Data)t.targetData).ColorLayer == block.ColorLayer)
			{
				targetDic[t] += newCount;
				OnTargetUpdated?.Invoke(block.BlockData.Sprite,targetDic[t]);
			}
		}
	}

	public bool CheckFinish()
	{
		return targetDic.All(t => t.Value <= 0);
	}
}