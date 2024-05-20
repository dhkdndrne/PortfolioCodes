using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public enum TextType
{
	Damage,
	Heal,
	CriDamage
}
public class DamageText : MonoBehaviour
{
    #region Inspector

	private TextMeshPro tmp;
	[SerializeField] private float duration;
	[SerializeField] private float moveSpeed;
	private PoolObject poolObject;

    #endregion

	#region Field

	private readonly Color DAMAGE_COLOR = Color.white;
	private readonly Color Heal_COLOR = new Color(0 / 255f, 200f / 255f, 24 / 255f);
	private readonly Color CRIDamage_COLOR = Color.red;

	private Color tempColor;
	private float timer;

	private bool isMoving;
	private float diableTime; //n초후 투명해지는 시간
	
    #endregion

	private void Awake()
	{
		tmp = GetComponent<TextMeshPro>();
		poolObject = GetComponent<PoolObject>();
	}

	public void InitText(string damage,TextType textType , Vector3 targetPos)
	{
		transform.position = new Vector3(targetPos.x + Random.Range(-0.25f, 0.25f), targetPos.y + Random.Range(0.3f, 1f), targetPos.z);

		tmp.color = textType switch
		{
			TextType.Heal => Heal_COLOR,
			TextType.CriDamage => CRIDamage_COLOR,
			_ => DAMAGE_COLOR
		};

		tmp.text = damage;
		tempColor = tmp.color;

		timer = duration;
		diableTime = timer * 0.5f;
		
		isMoving = true;
	}

	private void FixedUpdate()
	{
		if (!isMoving)
			return;

		timer -= Time.deltaTime;
		
		transform.position += Vector3.up * (moveSpeed * Time.deltaTime);
		tempColor.a = Mathf.Lerp(1, 0, (diableTime - timer) / diableTime);
		tmp.color = tempColor;
		
		if (timer <= 0)
		{
			isMoving = false;
			ObjectPoolManager.Instance.Despawn(poolObject);
		}
	}
}