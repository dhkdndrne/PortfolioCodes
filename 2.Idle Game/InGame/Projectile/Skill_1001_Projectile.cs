using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Skill_1001_Projectile : Projectile
{
	[SerializeField] private GameObject[] fxs;

	private Vector2[] point = new Vector2[4];
	private float t;

	public override void Init(UnitAI caster, UnitHp target, double damage, float speed)
	{
		base.Init(caster, target, damage, speed);

		point[0] = caster.transform.position;
		point[1] = PointSetting(caster.transform.position);
		point[2] = PointSetting(target.transform.position);
		point[3] = target.transform.position;

		SetFX(false);
		
		t = 0;
		Move().Forget();
	}

	protected override async UniTaskVoid Move()
	{
		while (t < 1)
		{
			transform.position = new Vector2(
				FourPointBezier(point[0].x, point[1].x, point[2].x, point[3].x),
				FourPointBezier(point[0].y, point[1].y, point[2].y, point[3].y));

			t += Time.deltaTime * speed;
			await UniTask.Yield();
		}

		SetFX(true);
		target.Hit(damage);
	}

	private Vector2 PointSetting(Vector2 origin)
	{
		float x, y;

		x = Random.Range(3f, 5f) * Mathf.Cos(Random.Range(0, 360) * Mathf.Deg2Rad) + origin.x;
		y = Random.Range(3f, 5f) * Mathf.Sin(Random.Range(0, 360) * Mathf.Deg2Rad) + origin.y;

		return new Vector2(x, y);
	}

	private float FourPointBezier(float a, float b, float c, float d)
	{
		return Mathf.Pow((1 - t), 3) * a
		       + Mathf.Pow((1 - t), 2) * 3 * t * b
		       + Mathf.Pow(t, 2) * 3 * (1 - t) * c
		       + Mathf.Pow(t, 3) * d;
	}

	private void SetFX(bool isBoom)
	{
		fxs[0].SetActive(!isBoom);
		fxs[1].SetActive(!isBoom);
		fxs[2].SetActive(isBoom);
	}
}