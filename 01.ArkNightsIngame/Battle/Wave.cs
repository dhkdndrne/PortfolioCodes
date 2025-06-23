using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
	public List<WayPointData> wayPoints = new List<WayPointData>();
	[SerializeField] public List<WaveGroup> waveList = new List<WaveGroup>();
}

[System.Serializable]
public class WaveGroup
{
	public int wayPointIndex;
	public float timeStamp;
	[SerializeField] public List<WaveData> waveDatas = new();
}

[System.Serializable]
public class WaveData
{
	public string enemyID;
	public int order;
	public float interval;
	//public bool waitForKill;
}
[System.Serializable]
public class WayPointData
{
	public int index;
	public List<WayPoint> points = new List<WayPoint>();
}