using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
	private Hex hex;
	private int hp;

	public int HP => hp;
	public Hex Hex => hex;

	public event Action<int> OnHpChanged; 
	
	public void SetHex(int x, int y)
	{
		hex = new Hex(x, y);
	}

	public void SetHP(int value)
	{
		hp += value;
		
		if (hp <= 0)
			hp = 0;
		
		OnHpChanged?.Invoke(hp);
	}
}