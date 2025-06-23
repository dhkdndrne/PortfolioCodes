using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class AttackRangeHandler
{
	/// <summary>
	/// 공통 코어: grid를 순회하면서 GridType이 CanAttack/Pivot인 셀을 origin 타일 기준 offset 해석 → validator로 검사 → 타일 반환
	/// </summary>
	private static List<Tile> GetTilesInternal(Tile onTile, GridType[,] grid, Func<int,int,bool> isValidTile)
	{
		var tileManager = GameManager.Instance.TileManager;
		var result = new List<Tile>();

		int rows = grid.GetLength(0);
		int cols = grid.GetLength(1);
		Vector2Int origin = onTile.TileIndex;
		Vector2Int pivot  = GetPivotIndex(grid);

		for (int y = 0; y < rows; y++)
		{
			for (int x = 0; x < cols; x++)
			{
				if (grid[y, x] is GridType.CanAttack or GridType.Pivot)
				{
					int targetX = origin.x + (x - pivot.x);
					int targetY = origin.y + (y - pivot.y);

					if (isValidTile(targetX, targetY))
						result.Add(tileManager.Tiles[targetY, targetX]);
				}
			}
		}
		return result;
	}

	/// <summary>
	/// 실제 공격 가능한 타일
	/// </summary>
	public static List<Tile> GetAttackableTiles(Tile onTile, GridType[,] grid)
	{
		return GetTilesInternal(onTile, grid, (tx, ty) => IsValidTile(tx, ty, GameManager.Instance.TileManager)
		);
	}

	/// <summary>
	/// 공격 범위 체크용
	/// </summary>
	public static List<Tile> GetAttackRangeTiles(Tile onTile, GridType[,] grid)
	{
		var tileManager = GameManager.Instance.TileManager;
		return GetTilesInternal(onTile, grid, (tx, ty) =>
				tx >= 0 && tx < tileManager.Tiles.GetLength(1) &&
				ty >= 0 && ty < tileManager.Tiles.GetLength(0)
		);
	}
	
	private static Vector2Int GetPivotIndex(GridType[,] grid)
	{
		int col = grid.GetLength(1);
		int row = grid.GetLength(0);
		for (int y = 0; y < row; y++)
		{
			for (int x = 0; x < col; x++)
			{
				if (grid[y, x] == GridType.Pivot)
					return new Vector2Int(x, y);
			}
		}
		return Vector2Int.zero;
	}
	private static bool IsValidTile(int x, int y,TileManager tileManager)
	{
		return x >= 0 && x < tileManager.Tiles.GetLength(1) &&
		       y >= 0 && y < tileManager.Tiles.GetLength(0) && tileManager.Tiles[y,x].TileType.HasFlag(TileType.Deployable);
	}
}