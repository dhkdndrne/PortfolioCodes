using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpSpriteHandler : MonoBehaviour
{
	[SerializeField] private SpriteRenderer[] sprites;

	public int MaxHp => sprites.Length;
	
	public void Init(int hp)
	{
		foreach (var sp in sprites)
			sp.gameObject.SetActive(false);

		for (int i = 0; i < hp; i++)
			sprites[i].gameObject.SetActive(true);
	}

	public void ChangeSprite(int hp)
	{
		sprites[hp].gameObject.SetActive(false);
	}
}