using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
	private Animator animator;
	private Rigidbody rb;
	
	[SerializeField, Range(0, 15f)] private float moveSpeed = 10f;
	[SerializeField, Range(5f, 15f)] private float rotSpeed = 6f;

	private Vector3 dir;
	private int IsMoveHash = Animator.StringToHash("IsMove");

	private void Awake()
	{
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		float hAxis = Input.GetAxisRaw("Horizontal");
		float vAxis = Input.GetAxisRaw("Vertical");

		dir = new Vector3(hAxis, 0, vAxis).normalized;
		
	}

	private void FixedUpdate()
	{
		Move();
	}

	private void Move()
	{
		if (dir == Vector3.zero)
		{
			animator.SetBool(IsMoveHash,false);
			return;
		}

		animator.SetBool(IsMoveHash,true);
		if (Mathf.Sign(transform.forward.x) != Mathf.Sign(dir.x) || Mathf.Sign(transform.forward.z) != Mathf.Sign(dir.z))
			transform.Rotate(0, 1, 0);

		transform.forward = Vector3.Lerp(transform.forward, dir, rotSpeed * Time.deltaTime);
		rb.MovePosition(transform.position + dir * moveSpeed * Time.deltaTime);

	}

}