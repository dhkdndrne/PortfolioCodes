using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class Stage : MonoBehaviour
{
	private int maxCost;
	private float costIncreaseTime;
	private float elapsedTime;
	private float costTime;
	private int waveIndex;

	private ObservableValue<int> life = new ObservableValue<int>(0);                                // 생명
	public ObservableValue<int> Cost { get; private set; } = new ObservableValue<int>(0);           // 현재 코스트
	public ObservableValue<int> CharacterLimit { get; private set; } = new ObservableValue<int>(0); // 최대 배치 가능 오퍼레이터 수
	private ObservableValue<int> killCount = new ObservableValue<int>(0);                           // 죽인 적 수
	private ObservableValue<float> sliderValue = new ObservableValue<float>(0);

	[Header("테스트용 데이터")]
	[SerializeField] private StageData stageData;
	[SerializeField] private EnemySpawner enemySpawner = new EnemySpawner();

	public int Life => life.Value;
	private bool isEndWave;
	
	public bool CheckClearStage()
	{
		bool isClear = isEndWave && enemySpawner.SpawnedEnemyCount == 0;

		if(isClear)
			Debug.Log("스테이지 끝");
		
		return isClear;
	}


	//todo 나중에 바꿔야함 한번에 여러개씩 나올 수 있음

	public void Init()
	{
		life.Value = stageData.LifePoint;
		Cost.Value = stageData.InitialCost;
		maxCost = stageData.MaxCost;

		costIncreaseTime = stageData.CostIncreaseTime;
		CharacterLimit.Value = stageData.CharacterLimit;

		var stageUIManager = FindAnyObjectByType<StageUIManager>();

		//ui 초기화 및 이벤트 등록
		stageUIManager.SubscribeEvent(life, Cost, maxCost, CharacterLimit, killCount,stageData.GetTotalEnemyCount(), sliderValue);
	}

	public void UpdateStage(float deltaTime)
	{
		FillCost(deltaTime);

		elapsedTime += deltaTime;
		var waveList = stageData.Wave.waveList;
		if (waveIndex < waveList.Count)
		{
			if (elapsedTime >= waveList[waveIndex].timeStamp)
			{
				//동시에 여러군데에서 나올수 있음
				foreach (var data in waveList[waveIndex].waveDatas)
				{
					enemySpawner.Spawn(stageData.Wave.wayPoints[waveList[waveIndex].wayPointIndex],data);
				}
				waveIndex++;
			}
		}
		else
		{
			isEndWave = true;
		}
	}

	private void FillCost(float deltaTime)
	{
		if (Cost.Value >= maxCost)
			return;

		costTime += deltaTime;
		sliderValue.Value = Mathf.Clamp01(costTime / costIncreaseTime);

		if (costTime >= costIncreaseTime)
		{
			// maxCost보다 많으면 코스트가 적어질때까지 대기
			// 게이지 100퍼센트 채운 상태에서 대기 하기위해서

			costTime -= costIncreaseTime;
			ChangeCostValue(1);
		}
	}

	public void ChangeCostValue(int amount)
	{
		Cost.Value += amount;

		if (Cost.Value <= 0) Cost.Value = 0;
		if (Cost.Value >= maxCost) Cost.Value = maxCost;
	}

	public void ReduceLife()
	{
		life.Value--;
	}
	public void AddKillCount()
	{
		killCount.Value++;
	}
}