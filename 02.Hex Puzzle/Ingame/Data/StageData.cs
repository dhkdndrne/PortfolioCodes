using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bam.Puzzle.Target;
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
	[field: SerializeField] private List<TargetToken> targetList;
	
	
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

	#endif

	[Serializable]
	public class TargetToken
	{
		public TargetContainer targetData;
		public int count;
	}
	
	[Serializable]
	public class EditorSBlockColorToken
	{
		public Hex hex;
		public ColorLayer colorLayer;
	}
	
	public enum CollectingTypes
	{
		Destroy,
		ReachBottom,
		Spread,
		Clear
	}
}