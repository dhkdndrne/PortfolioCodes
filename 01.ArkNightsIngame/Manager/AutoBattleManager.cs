using System;
using System.Collections.Generic;
using System.IO;
using Bam.Extensions;
using Bam.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoBattleManager : Singleton<AutoBattleManager>
{
	private List<TimelineEvent> eventTimeLine = new List<TimelineEvent>();
	private List<TimelineEvent> loadedTimeLine = new List<TimelineEvent>();
	
	public event Action<OperatorID> OnDeployAction;

	private TimeManager timeManager;
	// 이미 실행한 이벤트의 인덱스
	private int replayIndex;
	public ObservableValue<bool> IsReplayMode { get;} = new ObservableValue<bool>(false);
	
	protected override void Init()
	{
		LoadTimelineFromFile();
		
		if (loadedTimeLine.Count > 0)
		{
			StartReplay();
		}
		
		timeManager = TimeManager.Instance;
	}
	
	private void Update()
	{
		if(IsReplayMode.Value)
		{
			// 타임라인 리스트가 정렬되어 있다고 가정할 때, 현재 재생 프레임에 해당하는 이벤트들을 실행합니다.
			while (replayIndex < loadedTimeLine.Count &&
			       loadedTimeLine[replayIndex].frame <= timeManager.Frame)
			{
				Debug.LogError(timeManager.Frame);
				ExecuteTimelineEvent(loadedTimeLine[replayIndex]);
				replayIndex++;
			}
		}

		if (Input.GetKeyDown(KeyCode.A))
		{
			SaveTimelineToFile();
		}
	}

	// 재생을 시작하는 메서드 (로드된 타임라인이 있다고 가정)
	private void StartReplay()
	{
		// 만약 eventTimeLine이 로드되었다면, 프레임 순으로 정렬합니다.
		loadedTimeLine.Sort((a, b) => a.frame.CompareTo(b.frame));
		replayIndex = 0;
		IsReplayMode.Value = true;

		Debug.Log("리플레이 시작");
	}

	// 타임라인 이벤트를 실행하는 메서드
	private void ExecuteTimelineEvent(TimelineEvent timelineEvent)
	{
		// 이벤트 타입에 따라 분기합니다.
		switch (timelineEvent.eventType)
		{
			case ReplayEventType.Deploy:
				ExecuteDeployEvent(timelineEvent);
				break;
			case ReplayEventType.UseSkill:
				ExecuteUseSkillEvent(timelineEvent);
				break;
			case ReplayEventType.Retreat:
				ExecuteRetreatEvent(timelineEvent);
				break;
		}
	}

	private void ExecuteDeployEvent(TimelineEvent timelineEvent)
	{
		var gameManager = GameManager.Instance;
		DeployEventData data = timelineEvent.eventData as DeployEventData;

		var op = gameManager.OperatorManager.GetOperator(timelineEvent.operatorID);
		op.SetActive(true);

		//타일에 배치
		var tile = gameManager.TileManager.Tiles[data.TileIndex.y, data.TileIndex.x];
		tile.SetOperator(op);
		
		//오퍼레이터 위치 및 회전
		tile.SetUnitPosition(op.transform);
		op.Controller.Body.transform.rotation = Quaternion.Euler(0, data.Direction * 90, 0);
		op.SetAttackDir((OperatorDirection)data.Direction);

		op.Controller.DeployOperator();
		GameManager.Instance.DeployOperator(op);
		
		OnDeployAction?.Invoke(op.OperatorID);
	}

	private void ExecuteUseSkillEvent(TimelineEvent timelineEvent)
	{
		Debug.Log($"Use skill by operator: {timelineEvent.operatorID} at replay frame: {timelineEvent.frame}");
	
		var op = GameManager.Instance.OperatorManager.GetOperator(timelineEvent.operatorID);
		op.Controller.UseSkillManually();
	}

	private void ExecuteRetreatEvent(TimelineEvent timelineEvent)
	{
		Debug.Log($"Retreat operator: {timelineEvent.operatorID} at replay frame: {timelineEvent.frame}");
		
		var op = GameManager.Instance.OperatorManager.GetOperator(timelineEvent.operatorID);
		op.Controller.Retreat();
	}

	// 나중에 사용하기 위한 이벤트 기록 관련 메서드들...
	public void RecordEvent(TimelineEvent e)
	{
		eventTimeLine.Add(e);
		Debug.Log(e.ToString());
	}
	
	public void SaveTimelineToFile()
	{
		TimelineEventListWrapper wrapper = new TimelineEventListWrapper(eventTimeLine);
		string json = JsonUtility.ToJson(wrapper, true);
		string fileName = SceneManager.GetActiveScene().name;
		string filePath = Path.Combine(Application.persistentDataPath, fileName);
		File.WriteAllText(filePath, json);
		Debug.Log($"Timeline saved to {filePath}");
	}

	// 파일로부터 JSON을 읽어 타임라인 이벤트를 복원하는 메서드
	private void LoadTimelineFromFile()
	{
		string fileName = SceneManager.GetActiveScene().name;
		string filePath = Path.Combine(Application.persistentDataPath, fileName);
		if (File.Exists(filePath))
		{
			string json = File.ReadAllText(filePath);
			TimelineEventListWrapper wrapper = JsonUtility.FromJson<TimelineEventListWrapper>(json);
			loadedTimeLine = wrapper.events;
			Debug.Log($"Loaded {loadedTimeLine.Count} timeline events from {filePath}");
		}
		else
		{
			Debug.LogWarning($"File {filePath} does not exist!");
		}
	}
}

[Serializable]
public class TimelineEventListWrapper
{
	public List<TimelineEvent> events;

	public TimelineEventListWrapper(List<TimelineEvent> events)
	{
		this.events = events;
	}
}

