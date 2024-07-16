using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Bam.Extensions;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dice : MonoBehaviourPun, IPunObservable
{
	#region Inspector

	[SerializeField] private Transform[] eyes;
	[SerializeField] private Material[] specialDiceMatArr;

    #endregion

	#region Field

	private Define.DiceType diceType;

	private PhotonView pv;
	private Rigidbody rigidBody;
	private MeshRenderer meshRenderer;

	private Vector3 v3NetworkPos;    //지연보상 위치정보
	private Quaternion qtNetworkRot; //지연보상 회전정보

	private float stopThreshold = 0.001f;
    #endregion

	private void Awake()
	{
		pv = GetComponent<PhotonView>();
		meshRenderer = GetComponent<MeshRenderer>();
		rigidBody = GetComponent<Rigidbody>();
	}
	private void FixedUpdate()
	{
		if (!pv.IsMine)
		{
			rigidBody.position = Vector3.MoveTowards(rigidBody.position, v3NetworkPos, Time.fixedDeltaTime);
			rigidBody.rotation = Quaternion.RotateTowards(rigidBody.rotation, qtNetworkRot, Time.fixedDeltaTime * 200f);
		}
	}

	public void SetActivate(bool isActive) => pv.RPC(nameof(RPC_SetActivate), RpcTarget.All, isActive);
	public void ChangeDiceColor(Define.DiceType type) => pv.RPC(nameof(RPC_ChangeDiceColor), RpcTarget.All, type);

	public async UniTask<(int,Define.DiceType)> Roll()
	{
		rigidBody.position = new Vector3(Random.Range(-15, 15), 45, Random.Range(-15, 15));
		rigidBody.rotation = Quaternion.Euler(Random.Range(-90f, 90f), Random.Range(-90f, 90f), Random.Range(-90f, 90f));
		rigidBody.AddForce(new Vector3(Random.Range(0, 15), -40, Random.Range(0, 15)), ForceMode.VelocityChange);
		rigidBody.angularVelocity = Random.insideUnitSphere * 9999;

		await UniTask.WaitUntil(() => Extensions.Abs(rigidBody.velocity.magnitude) < stopThreshold && Extensions.Abs(rigidBody.angularVelocity.magnitude) < stopThreshold);

		float max = -999;
		int index = 0;
		foreach (var eye in eyes)
		{
			if (eye.position.y > max)
			{
				max = eye.position.y;
				index = eye.GetSiblingIndex() + 1;
			}
		}

		return (index, diceType);
	}
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(rigidBody.position);
			stream.SendNext(rigidBody.rotation);
			stream.SendNext(rigidBody.velocity);
		}
		else
		{
			v3NetworkPos = (Vector3)stream.ReceiveNext();
			qtNetworkRot = (Quaternion)stream.ReceiveNext();
			rigidBody.velocity = (Vector3)stream.ReceiveNext();

			float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
			v3NetworkPos += rigidBody.velocity * lag;
		}
	}

	#region RPC

	[PunRPC]
	private void RPC_ChangeDiceColor(Define.DiceType type)
	{
		diceType = type;
		if (diceType == Define.DiceType.Special)
		{
			meshRenderer.materials = specialDiceMatArr;
		}
	}

	[PunRPC]
	private void RPC_SetActivate(bool isActive) => gameObject.SetActive(isActive);

    #endregion
}