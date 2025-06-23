using System;
using System.Collections.Generic;
using Bam.Singleton;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TileManager : MonoBehaviour
{
	private Tile[,] tiles;

	public Tile[,] Tiles => tiles;

	private Transform tilesParent;
	
	[SerializeField] private List<Tile> highlightedTiles = new List<Tile>();

	private void Awake()
	{
		tilesParent = GameObject.Find("Tile").transform;
	}

	// 타일 데이터 저장.
	private void Start()
	{
		Init();
	}

	private void Init()
	{
		int childCnt = tilesParent.childCount;

		int col = 0;
		int row = 0;
		
		Queue<Tile> q = new Queue<Tile>();
		
		for (int i = 0; i < childCnt; i++)
		{
			var tile = tilesParent.GetChild(i).GetComponent<Tile>();

			int x = (int)tile.transform.position.x;
			int y = (int)tile.transform.position.z;

			col = Math.Max(col,x);
			row = Math.Max(row,y);
			
			tile.SetTileIndex(x, y);
			tile.name = $"{y} / {x}";
			
			q.Enqueue(tile);
		}
		
		tiles = new Tile[row + 1,col + 1];

		while (q.Count > 0)
		{
			var tile = q.Dequeue();
			tiles[tile.TileIndex.y,tile.TileIndex.x] = tile;
		}
	}

	public void ShowPlaceableTile(Operator op)
	{
		highlightedTiles.Clear();
		
		foreach (var t in tiles)
		{
			if(!t.TileType.HasFlag(TileType.Deployable))
				continue;
			
			if (t.UnitOnTile != null) 
				continue;
			
			//오퍼레이터 공격 타입에 맞는 배치 가능 타일 추가
			switch (op.AtkType)
			{
				case Operator_AtkType.Melee when t.HeightType is HeightType.Highland:
				case Operator_AtkType.Ranged when t.HeightType is HeightType.Lowland:
					continue;
				default:
					highlightedTiles.Add(t);
					t.StartPlaceableAnim();
					break;
			}
		}
	}

	public void DisablePlaceableTile()
	{
		foreach (var t in highlightedTiles)
		{
			t.StopPlaceableAnim();
		}
	}

	public Tile GetTile(Vector3 pos)
	{
		return tiles[(int)pos.z, (int)pos.x];
	}
}