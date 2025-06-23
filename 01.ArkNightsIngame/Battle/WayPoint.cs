using UnityEngine;
using System;

[Serializable]
public class WayPoint
{
	[SerializeField] private WayPointType type;
	[SerializeField] private Vector3 position;
	[SerializeField] private float waitTime;

	public WayPointType Type => type;
	public Vector3 Position => position;
	public float WaitTime => waitTime;

	public WayPoint(WayPointType type, Vector3 position, float waitTime = 0f)
	{
		this.type = type;
		this.position = position;
		this.waitTime = waitTime;
	}
}