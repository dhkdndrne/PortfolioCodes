using System.Collections.Generic;
[System.Serializable]
public struct Hex
{
	#region Field

	public int x;
	public int y;

    #endregion

	public Hex(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
	
	/// <summary>
	///	순서		  Index
	/// 위			0
	/// 왼쪽 위		1
	/// 왼쪽 아래	2
	/// 아래			3
	/// 오른쪽 아래	4
	/// 오른쪽 위	5
	/// </summary>
	private static List<Hex> oddDirections = new List<Hex>()
	{
		new Hex(0, 1), new Hex(-1, 0), new Hex(-1, -1), new Hex(0, -1), new Hex(1, -1), new Hex(1, 0)
	};
	
	/// <summary>
	///	순서		  Index
	/// 위			0
	/// 왼쪽 위		1
	/// 왼쪽 아래	2
	/// 아래			3
	/// 오른쪽 아래	4
	/// 오른쪽 위	5
	/// </summary>
	private static List<Hex> directions = new List<Hex>()
	{
		new Hex(0, 1), new Hex(-1, 1), new Hex(-1, 0),  new Hex(0, -1),new Hex(1, 0), new Hex(1, 1),
	};

	/// <summary>
	/// 현재 위치에서 HewWay 방향의 Hex 반환
	/// </summary>
	/// <param name="hex"></param>
	/// <param name="way"></param>
	/// <returns></returns>
	public static Hex GetHexByWay(Hex hex,HexWay way)
	{
		Hex newHex = GetHexList(hex)[(int)way];
		return new Hex(hex.x + newHex.x, hex.y + newHex.y);
	}

	private static bool IsOddRow(Hex hex)
	{
		return hex.x % 2 == 1;
	}
	
	public static List<Hex> GetHexList(Hex hex)
	{
		return IsOddRow(hex) ? oddDirections : directions;
	}
	
	public static Hex operator -(Hex a, Hex b)
	{
		return new Hex(a.x - b.x, a.y - b.y);
	}

	public static Hex operator +(Hex a, Hex b)
	{
		return new Hex(a.x + b.x, a.y + b.y);
	}
	
	public static bool operator ==(Hex a, Hex b)
	{
		return a.x == b.x && a.y == b.y;
	}
	
	public static bool operator !=(Hex a, Hex b)
	{
		return a.x != b.x || a.y != b.y;
	}
}

