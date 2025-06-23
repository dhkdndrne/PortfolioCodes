using System;
using UnityEngine;
[Serializable]
public abstract class TimelineEventData { }

// 배치 이벤트에 필요한 추가 데이터 (예: 배치 방향 등)
[Serializable]
public class DeployEventData : TimelineEventData
{
	[SerializeField]
	private Vector2Int tileIndex;
	public Vector2Int TileIndex => tileIndex;

	[SerializeField]
	private int direction;
	public int Direction => direction;
    
	public DeployEventData(int direction, Vector2Int tileIndex)
	{
		Debug.Log((OperatorDirection) direction);
		this.direction = direction;
		this.tileIndex = tileIndex;
	}
}

// 타임라인 이벤트 클래스는 기본 정보와 추가 데이터를 모두 담을 수 있도록 함
[Serializable]
public class TimelineEvent
{
	public OperatorID operatorID;
	public int frame;
	public ReplayEventType eventType;
	[SerializeReference] public TimelineEventData eventData; // 추가 이벤트 데이터 (없을 수도 있음)

	// 기본 생성자 (추가 데이터가 없는 경우)
	public TimelineEvent(OperatorID operatorID, int frame, ReplayEventType eventType)
	{
		this.operatorID = operatorID;
		this.frame = frame;
		this.eventType = eventType;
	}
    
	// 추가 데이터가 있는 경우의 생성자
	public TimelineEvent(OperatorID operatorID, int frame, ReplayEventType eventType, TimelineEventData eventData)
		: this(operatorID, frame, eventType)
	{
		this.eventData = eventData;
	}
    
	public override string ToString()
	{
		return $"operatorID: {operatorID}, frame: {frame}, eventType: {eventType}";
	}
}