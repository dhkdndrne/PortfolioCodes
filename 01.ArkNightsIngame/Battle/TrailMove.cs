using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TrailMove : MonoBehaviour
{
	private bool isInit;
	private int index;
	private List<WayPoint> waypoints;
	private const int MOVE_SPEED = 5;

	private void OnEnable()
	{
		isInit = false;
		index = 0;
	}

	public void SetWaypoint(List<WayPoint> waypoints)
	{
		isInit = true;
		this.waypoints = waypoints;
	}
	
	public async UniTask Move()
	{
		while (index < waypoints.Count)
		{
			var targetPosition = waypoints[index].Position;
			//targetPosition.y = 0.65f;
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, MOVE_SPEED * CustomTime.deltaTime);
		
			var distance = Vector3.Distance(transform.position, targetPosition);
			if (distance <= 0.01f)
			{
				index++;
			}
			
			await UniTask.Yield();
		}
		
		gameObject.SetActive(false);
	}
	
	
}
