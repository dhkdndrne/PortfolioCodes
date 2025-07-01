using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bam.Singleton;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class Board : MonoBehaviour
{
	private Transform blockHolder;
	private Transform cellHolder;

	private Cell[,] cells;
	private Block[,] blocks;
	private StageData stageData;
	private BoardShuffleSystem shuffleSystem;

	[SerializeField] private bool showCellIndex;
	[field: SerializeField] public List<Cell> SpawnCellList { get; private set; } = new();

	public int Col => stageData.Col;
	public int Row => stageData.Row;
	public float ColGap => stageData.ColGap;
	public float RowGap => stageData.RowGap;
	public Block GetBlock(int x, int y) => blocks[y, x];
	public Block GetBlock(Hex hex) => blocks[hex.y, hex.x];
	public Cell GetCell(int x, int y) => cells[y, x];
	public Cell GetCell(Hex hex) => cells[hex.y, hex.x];

	public void InitBoard(StageData data)
	{
		stageData = data;

		CreateHolder();
		SetBoard();

		(shuffleSystem ??= new BoardShuffleSystem(this)).ValidateStageShuffle();
	}

	public void CheckCanMatch() => shuffleSystem.CheckMatchingBlocks();

	private void CreateHolder()
	{
		if (cellHolder != null)
			DestroyImmediate(cellHolder.gameObject);
		if (blockHolder != null)
			DestroyImmediate(blockHolder.gameObject);

		GameObject sh = new GameObject("SocketHolder");
		cellHolder = sh.transform;
		cellHolder.transform.localPosition = Vector3.zero;

		GameObject bh = new GameObject("BlockHolder");
		blockHolder = bh.transform;
		blockHolder.transform.localPosition = Vector3.zero;
	}
	private void SetBoard()
	{
		cells = new Cell[Row, Col];
		blocks = new Block[Row, Col];

		InitTargets();

		for (int y = 0; y < Row; y++)
		{
			for (int x = 0; x < Col; x++)
			{
				InitCell(x, y);
				InitBlock(x, y);
			}
		}

		float yCenter = (Row - 1) * RowGap / 2f;
		float xCenter = (Col - 1) * ColGap / 2f;

		cellHolder.position = new Vector3(-xCenter, -yCenter, cellHolder.position.z);
		blockHolder.position = new Vector3(-xCenter, -yCenter, blockHolder.position.z);

		blockHolder.gameObject.SetActive(false);
		cellHolder.gameObject.SetActive(false);
	}

	private void InitTargets()
	{
		var targetList = stageData.GetTargetList();

		foreach (var targetToken in targetList)
		{
			if (targetToken.tokenList.Count == 0) continue;

			var targetType = targetToken.targetData.TargetObjectType;
			int count = targetToken.count; // 타겟의 개수
			Transform parent = targetType is TargetObjectType.Block ? blockHolder : cellHolder;

			for (int i = 0; i < count; i++)
			{
				Hex hex = targetToken.tokenList[i].hex;

				var targetData = (Target_Block_Data)targetToken.targetData;
				var obj = Instantiate(((Target_Block_Data)targetToken.targetData).Prefab, parent);
				var block = obj.GetComponent<Block>();

				var newBlockData = ScriptableObject.CreateInstance<BlockData>();
				var blockData = targetData.BlockData;

				// Reflection을 사용해 필드 값 복사
				Util.CopyFields(blockData, newBlockData);

				//체력 재설정
				blockData.HP = targetToken.tokenList[i].hp;

				block.SetData(targetData.BlockData);
				SetBlockTransform(block, hex.x, hex.y);
			}
		}
	}

	private void InitCell(int x, int y)
	{
		var cellObject = ObjectPoolManager.Instance.Spawn("Cell");

		//부모 및 위치
		cellObject.transform.SetParent(cellHolder);
		cellObject.transform.localPosition = IndexToLocalPos(x, y);

		//셀 오브젝트
		var cell = cellObject.GetComponent<Cell>();
		cell.SetHex(x, y);
		cells[y, x] = cell;

		cell.Shadow.SetActive(false);
		cell.tmp.enabled = true;

		if (showCellIndex)
			cell.tmp.text = $"[{y},{x}]";

		var cellData = stageData.GetCellData(x, y);

		if (cellData.cellType is CellType.Spawn)
		{
			SpawnCellList.Add(cell);
		}

		if (cellData.cellType is not CellType.Basic)
			cell.gameObject.SetActive(false);

	}

	private void InitBlock(int x, int y)
	{
		var cellData = stageData.GetCellData(x, y);
		if (cellData.cellType is not CellType.Basic)
			return;

		if (GetBlock(x, y) != null) return;

		CreateNewBlock(x, y);
	}

	public void CreateNewBlock(int x, int y)
	{
		BlockData blockData = stageData.GetBlockData(x, y);

		Block block = blockData switch
		{
			not null => BlockSpawner.Instance.SpawnBlock(blockData, new Hex(x, y)),
			null => BlockSpawner.Instance.SpawnRandomBlock()
		};

		SetBlockTransform(block, x, y);
	}

	public void CreateNewItemBlock(ReservedSBlockData sBlockData)
	{
		var block = BlockSpawner.Instance.SpawnSpecialBlock(sBlockData);
		var hex = sBlockData.Hex;
		SetBlockTransform(block, hex.x, hex.y);
	}

	private void SetBlockTransform(Block block, int x, int y)
	{
		block.SetHex(x, y);
		blocks[y, x] = block;

		var blockTr = block.transform;
		blockTr.SetParent(blockHolder);
		blockTr.localPosition = IndexToLocalPos(x, y);
	}

	public IEnumerable<Block> GetBlockEnumerable()
	{
		var row = Row;
		var col = Col;

		for (int y = 0; y < row; y++)
		{
			for (int x = 0; x < col; x++)
			{
				if (!IsValidIndex(x, y)) continue;
				var block = GetBlock(x, y);

				if (block == null)
					continue;

				yield return block;
			}
		}
	}
	public IEnumerable<Cell> GetCellEnumerable()
	{
		var row = Row;
		var col = Col;

		for (int y = 0; y < row; y++)
		{
			for (int x = 0; x < col; ++x)
			{
				if (!IsValidIndex(x, y)) continue;
				var cell = GetCell(x, y);
				if (cell == null) continue;

				yield return cell;
			}
		}
	}
	public Vector3 IndexToWorldPos(Hex hex)
	{
		int x = hex.x;
		int y = hex.y;
		return blockHolder.transform.TransformPoint(IndexToLocalPos(x, y));
	}
	/// <summary>
	/// 인덱스를 통해 위치 정보로 변환
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	private Vector3 IndexToLocalPos(int x, int y)
	{
		float nx = x * ColGap;
		float ny = y * RowGap + (x % 2 == 1 ? -RowGap * 0.5f : 0);

		return new Vector3(nx, ny, 0);
	}
	public Hex WorldPosToHex(Vector3 pos)
	{
		var rowGap = RowGap;
		var colGap = ColGap;

		Vector3 localPos = cellHolder.transform.InverseTransformPoint(pos);
		int nx = Mathf.RoundToInt(localPos.x / colGap);

		float ny = localPos.y / rowGap;

		if (nx % 2 == 1)
		{
			ny += rowGap * 0.5f;
		}
		return new Hex(nx, Mathf.RoundToInt(ny));
	}

	public bool IsValidIndex(int x, int y)
	{
		return stageData.GetCellData(x, y).cellType is CellType.Basic;
	}
	public bool IsValidIndex(Hex hex)
	{
		return stageData.GetCellData(hex.x, hex.y).cellType is CellType.Basic;
	}
	public bool IsIndexOutOfRange(Hex hex)
	{
		return hex.x < 0 || hex.y < 0 || hex.x >= Col || hex.y >= Row;
	}

	public bool HasSpecialBlock()
	{
		return GetBlockEnumerable().Any(block => block.BlockData as SpecialBlockData != null);
	}

	public void ExchangeBlockInfo(Block b1, Block b2)
	{
		var temp = b1;
		var hex = b1.Hex;

		blocks[b1.Hex.y, b1.Hex.x] = b2;
		blocks[b2.Hex.y, b2.Hex.x] = temp;

		b1.SetHex(b2.Hex.x, b2.Hex.y);
		b2.SetHex(hex.x, hex.y);
	}

	public void SetBlock(Hex hex, Block block)
	{
		blocks[hex.y, hex.x] = block;
		block?.SetHex(hex.x, hex.y);
	}

	public void RemoveBlock(Hex blockHex)
	{
		var block = blocks[blockHex.y, blockHex.x];
		if (block != null)
			BlockSpawner.Instance.DeSpawnBlock(block);

		blocks[blockHex.y, blockHex.x] = null;
	}

	/// <summary>
	/// 한번에 보여줌
	/// </summary>
	/// <param name="hashSet"></param>
	public async UniTask ShowTargetHighlightAll(HashSet<Block> hashSet)
	{
		foreach (var b in hashSet)
		{
			var cell = GetCell(b.Hex);
			cell.Shadow.SetActive(false);
		}
		await UniTask.Delay(TimeSpan.FromSeconds(0.35f));
	}
	public void SetCellShadow(bool active)
	{
		foreach (var cell in GetCellEnumerable())
		{
			if (cell.gameObject.activeSelf)
				cell.Shadow.SetActive(active);
		}
	}

	public async UniTask PlayBoardAnim()
	{
		blockHolder.gameObject.SetActive(true);
		cellHolder.gameObject.SetActive(true);

		List<UniTask> tasks = new List<UniTask>();
		foreach (var block in GetBlockEnumerable())
		{
			Tween tween = block.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.Linear).From(Vector3.zero);
			tasks.Add(tween.ToUniTask());
		}

		foreach (var src in GetCellEnumerable())
		{
			Tween tween = src.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.Linear).From(Vector3.zero);
			tasks.Add(tween.ToUniTask());
		}

		await UniTask.WhenAll(tasks);
	}
}