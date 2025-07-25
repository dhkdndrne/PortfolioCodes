using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using EditorUtility = UnityEditor.EditorUtility;
using Random = UnityEngine.Random;
using Undo = UnityEditor.Undo;


[CreateAssetMenu(menuName = "SO/Stage Data", fileName = "New Stage Data")]
[Serializable]
public class StageData : ScriptableObject
{
	[field: SerializeField] public int StageNum { get; private set; }
	[field: SerializeField] public int MoveCnt { get; private set; }
	[field: SerializeField] public BlockData[] AppearBlocks { get; private set; }

	[field: SerializeField] public int Col { get; private set; } = 7;
	public float ColGap = 1.11f;

	[field: SerializeField] public int Row { get; private set; } = 7;
	public float RowGap = 1.28f;

	[SerializeField] private List<CellData> cells = new List<CellData>();
	[SerializeField] private List<BlockData> blocks = new List<BlockData>();
	
	public List<EditorSBlockColorToken> sBlockColorTokens = new List<EditorSBlockColorToken>();
	[field: SerializeField] private List<TargetToken> targetList = new();
	
	
	/// <summary>
	/// 리스트에 있는 ColorLayer를 제외한 색 중 랜덤한 blockData 반환
	/// </summary>
	/// <param name="colorLayers"></param>
	/// <returns></returns>
	public BlockData GetBlockDataExceptColor(List<ColorLayer> colorLayers)
	{
		var availableColors = AppearBlocks
			.Where(data => !colorLayers.Contains(data.ColorLayer) && data.ColorLayer != ColorLayer.None)
			.Select(data => data.ColorLayer)
			.Distinct()
			.ToList();

		// 랜덤으로 색상 변경
		var newColor = availableColors[Random.Range(0, availableColors.Count)];
		return AppearBlocks.Where(data => data.ColorLayer == newColor).FirstOrDefault();
	}
	public BlockData GetRandomBlockData() => AppearBlocks[Random.Range(0, AppearBlocks.Length)];
	public BlockData GetBlockData(int x, int y)
	{
		return blocks[GetSingleIndex(x, y)];
	}
	public CellData GetCellData(int x, int y)
	{
		return cells[GetSingleIndex(x, y)];
	}
	
	public List<TargetToken> GetTargetList()=> targetList;
	private int GetSingleIndex(Hex hex) => hex.y * Col + hex.x;
	private int GetSingleIndex(int x, int y) => y * Col + x;
	
	#if UNITY_EDITOR

	public void SetCol(int col) => Col = col;
	public void SetRow(int row) => Row = row;

	private readonly string BasicCellPath = "Assets/09.Data/Cell/Basic.asset";
	public void SetBoardSize()
	{
		ValidateSize(ref cells, AssetDatabase.LoadAssetAtPath<CellData>(BasicCellPath));
		ValidateSize(ref blocks, null);
		Debug.LogSuccess("보드 크기 설정");
	}

	public void SetBlockData(Hex hex, BlockData blockData)
	{
		blocks[GetSingleIndex(hex)] = blockData;
		Debug.Log($"{hex.y} / {hex.x} = {blockData}");
		EditorUtility.SetDirty(this);
	}

	public void SetCellData(Hex hex, CellData cellData)
	{
		cells[GetSingleIndex(hex)] = cellData;
		Debug.Log($"{hex.y} / {hex.x} = {cellData}");
		EditorUtility.SetDirty(this);
	}
	
	private void ValidateSize<T>(ref List<T> list, T defaultValue)
	{
		if (list == null) list = new List<T>();

		int size = list.Count;
		while (size++ < Row * Col)
		{
			list.Add(defaultValue);
		}

		size = list.Count;
		while (size-- > Row * Col)
		{
			list.RemoveAt(list.Count - 1);
		}
	}

	public void AddOrUpdateBlockToken(Target_Block_Data blockData, Hex hex, int hp)
	{
		var existingToken = targetList.FirstOrDefault(t => t.targetData == blockData);
		if (existingToken == null)
		{
			targetList.Add(new TargetToken
			{
				targetData = blockData,
				count = 1,
				tokenList = new List<Target_Block_Token>
				{
					new Target_Block_Token { hp = hp, hex = hex }
				}
			});
		}
		else
		{
			var blockToken = existingToken.tokenList.FirstOrDefault(t => t.hex == hex);
			if (blockToken != null)
			{
				// 이미 존재하면 hp만 갱신
				blockToken.hp = hp;
			}
			else
			{
				existingToken.tokenList.Add(new Target_Block_Token { hp = hp, hex = hex });
				existingToken.count++;
			}
		}
	}
	public void RemoveBlockToken(Hex hex)
	{
		// hex를 가진 블록 토큰을 찾아 제거
		foreach (var token in targetList)
		{
			var blockToken = token.tokenList.FirstOrDefault(t => t.hex == hex);
			if (blockToken != null)
			{
				token.tokenList.Remove(blockToken);
				token.count--;
				if (token.count <= 0)
				{
					targetList.Remove(token);
				}
				break;
			}
		}
	}
	#endif
	
	[Serializable]
	public class EditorSBlockColorToken
	{
		public Hex hex;
		public ColorLayer colorLayer;
	}
}
[Serializable]
public class TargetToken
{
	public TargetData targetData;
	public List<Target_Block_Token> tokenList;
	public int count;
}

[Serializable]
public class Target_Block_Token
{
	public int hp;
	public Hex hex;
}