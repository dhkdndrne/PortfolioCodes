using System;
using System.Collections;
using System.Collections.Generic;
using Bam.Singleton;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;


public class StageManager : Singleton<StageManager>
{
	[SerializeField] private EnemySpawnSystem enemySpawnSystem = new EnemySpawnSystem();
	[SerializeField] private FadeSystem fadeSystem = new FadeSystem();

	public Action onInitStage;

	public List<UnitHp> SpawnedEnemyList { get; private set; }
	public Stage CurStage { get; private set; }

	private int level;
	private bool isStageChanging;

	private SaveData_Stage deadStageInfo;
	public ReactiveProperty<bool> IsInfinityStage { get; private set; } = new();
	public Subject<Unit> OnDeadSubject { get; private set; } = new();

	protected override void Awake()
	{
		base.Awake();
		
		SpawnedEnemyList = new List<UnitHp>();
		CurStage = new Stage();

		Initializator.Instance.onSecondInit += Init;
		onInitStage += () => InitStage().Forget();

		var playerData = DataManager.Instance.PlayerData;
		IsInfinityStage.Value = playerData.isInfinityStage;
		deadStageInfo = playerData.stageData;
		
		Observable.EveryUpdate()
			.Where(_ => SpawnedEnemyList.Count == 0 && !isStageChanging)
			.Subscribe(_ =>
			{
				if (!IsInfinityStage.Value)
				{
					NextWave();
					DataManager.Instance.SaveData();
					UtilClass.DebugLog("웨이브 증가");
				}
				else
				{
					if (deadStageInfo.stageLevel == level && deadStageInfo.stageWave == CurStage.Wave.Value)
					{
						IsInfinityStage.Value = false;
						NextWave();
						return;
					}
					
					onInitStage?.Invoke();
					isStageChanging = true;
				}
			});

		OnDeadSubject.Subscribe(_ =>
		{
			IsInfinityStage.Value = true;
			isStageChanging = true;
			
			deadStageInfo.stageWave = CurStage.Wave.Value;
			deadStageInfo.stageLevel = level;
			
			foreach (var enemy in SpawnedEnemyList)
			{
				ObjectPoolManager.Instance.Despawn(enemy.GetComponent<PoolObject>());
			}
			SpawnedEnemyList.Clear();
			DataManager.Instance.SaveData();
			
			onInitStage?.Invoke();
		}).AddTo(this);
	}

	private void Init()
	{
		enemySpawnSystem.Init();
		CurStage.Init();

		level = DataManager.Instance.PlayerData.stageData.stageLevel;
		enemySpawnSystem.SpawnEnemy(CurStage.EnemyListDic[CurStage.Wave.Value]);
	}
	private void NextWave()
	{
		if (CurStage.CheckGoNextStage())
		{
			isStageChanging = true;
			
			onInitStage?.Invoke();
			UtilClass.DebugLog("스테이지 업");
		}
		else
		{
			enemySpawnSystem.SpawnEnemy(CurStage.EnemyListDic[CurStage.Wave.Value]);
		}
	}

	private async UniTaskVoid InitStage()
	{
		await fadeSystem.FadeIn();
		Player.Instance.PlayerUnit.transform.position = new Vector3(-1.92f, -4.62f, 0);

		level = IsInfinityStage.Value ? level : level + 1 > DataManager.Instance.GetStageData.Count ? level : level + 1;
		DataManager.Instance.PlayerData.stageData.stageLevel = level;
		CurStage.Init();

		await fadeSystem.FadeOut();
        
		enemySpawnSystem.SpawnEnemy( CurStage.EnemyListDic[CurStage.Wave.Value]);
		isStageChanging = false;
	}
}