using System;
using System.Collections;
using System.Collections.Generic;
using Bam.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
	private Player target;
	private Enemy unitBase;

	private Animator animator;
	private SkinnedMeshRenderer skinnedRenderer;
	private StateMachine stateMachine;

	#region Property

	public Player Target { get => target; }
	public Animator Animator { get => animator; }
	public Enemy UnitBase { get => unitBase; }
	public SkinnedMeshRenderer SkinnedRenderer { get => skinnedRenderer; }

    #endregion


	private void Awake()
	{
		unitBase = GetComponent<Enemy>();
		animator = GetComponent<Animator>();
		skinnedRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
		stateMachine = GetComponent<StateMachine>();
	}

	public void Init()
	{
		stateMachine.Init();
		target = CreatureManager.Instance.Player;
		
		unitBase.Init(this);
	}

	public bool CheckTargetInAttackRange()
	{
		return Vector3.SqrMagnitude(transform.position - target.transform.position) <= Extensions.Pow(2f, 2);
	}

	private void Update()
	{
		unitBase.UpdateCoolTime(Time.deltaTime);
	}
}