using System;
using UnityEngine;

[Serializable]
public class Grid
{
	[SerializeField] private int x, y;

	public Grid(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
	
	public int X => x;
	public int Y => y;
}