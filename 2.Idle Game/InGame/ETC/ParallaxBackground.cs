using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
	[SerializeField] private Transform target;
	[SerializeField] private float speed;
	[SerializeField] private Vector3 moveDirection;

	private Vector3 originPos;
	
	private readonly float SCROLL_AMOUNT = 40f;
    
	private bool isMove;

	private void Awake()
	{
		Initializator.Instance.onAfterPlayerInit += Init;
		StageManager.Instance.onInitStage += ResetPosition;

		originPos = transform.position;
	}

	private void Init()
	{
		speed += Player.Instance.PlayerUnit.unitBase.Stat.MoveSpeed;
		
		var stateMachine = Player.Instance.PlayerUnit.GetComponent<StateMachine>();
		
		Observable.EveryUpdate()
			.Select(_ => stateMachine.CurrentState is State_Move) // State_Move 여부를 검사
			.DistinctUntilChanged()                               // 이전 값과 현재 값이 다를 때만 통과
			.Subscribe(isMoving =>
			{
				isMove = isMoving;
			});
		
		Observable.EveryUpdate().Where(_ => isMove).Subscribe(_ =>
		{
			transform.position += moveDirection * (speed * Time.deltaTime);
			if (transform.position.x <= -SCROLL_AMOUNT)
			{
				transform.position = target.position - (moveDirection * SCROLL_AMOUNT);
			}
            
		}).AddTo(this);
	}

	private void ResetPosition()
	{
		transform.position = originPos;
	}
	
}