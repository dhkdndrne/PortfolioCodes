using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Puzzle.Ingame;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class Board_Edit : ObjectSingleton<Board_Edit>
{
	public StageData stageData;

	private Cell[,] cells;
	private Block[,] blocks;
	private GameObject[,] targets;
	
	private Transform cellHolder;
	private Transform blockHolder;

	private List<Cell> cellList = new List<Cell>();

	private const string BLOCK_PREFABLIST_PATH = "Assets/09.Data/Editor BlockPrefabList.asset";
	private EditorBlockPrefabList editorBlockPrefabList;

	public int Col => stageData.Col;
	public int Row => stageData.Row;
	public float ColGap => stageData.ColGap;
	public float RowGap => stageData.RowGap;
	
	public Board_Edit()
	{
		editorBlockPrefabList = AssetDatabase.LoadAssetAtPath<EditorBlockPrefabList>(BLOCK_PREFABLIST_PATH);
	}

	public void SetStageData(StageData data)
	{
		stageData = data;
		cellList.Clear();
		stageData?.SetBoardSize();
	}

	public void SetBoard()
	{
		cells = new Cell[Row, Col];
		blocks = new Block[Row, Col];
		targets = new GameObject[Row, Col];

		DestroyHolder();
		CreateHolder();
		
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

		GameObject.FindObjectOfType<CameraController>().FitCamera(Row, RowGap);
	}

	public void DestroyHolder()
	{
		blockHolder = GameObject.Find("BlockHolder")?.transform;
		cellHolder = GameObject.Find("SocketHolder")?.transform;

		if (cellHolder != null)
			Object.DestroyImmediate(cellHolder.gameObject);

		if (blockHolder != null)
			Object.DestroyImmediate(blockHolder.gameObject);
	}
	private void CreateHolder()
	{
		if (cellHolder == null)
		{
			GameObject o = new GameObject("SocketHolder");
			cellHolder = o.transform;
			cellHolder.transform.localPosition = Vector3.zero;
		}
		if (blockHolder == null)
		{
			GameObject o = new GameObject("BlockHolder");
			blockHolder = o.transform;
			blockHolder.transform.localPosition = Vector3.zero;
		}
	}

	private void InitCell(int x, int y)
	{
		var cellObject = GameObject.Instantiate(stageData.GetCellData(x, y).prefab, cellHolder, true);

		//부모 및 위치
		cellObject.transform.localPosition = IndexToLocalPos(x, y);

		//셀 데이터
		var cell = cellObject.GetComponent<Cell>();
		cell.SetHex(x, y);
		cells[y, x] = cell;

		cell.tmp.enabled = true;

		var stageCellData = stageData.GetCellData(x, y);
		switch (stageCellData.cellType)
		{
			case CellType.Basic:
				cell.tmp.text = $"[{y},{x}]";
				break;

			case CellType.Spawn:
				cell.tmp.text = "[Spawn]";
				break;
		}

		if (stageCellData.cellType is CellType.None)
			cell.gameObject.SetActive(false);
	}

	private void InitBlock(int x, int y)
	{
		var blockData = stageData.GetBlockData(x, y);
		if (blockData == null) return;

		SpecialBlockType blockType = blockData is not SpecialBlockData sBlockData ? SpecialBlockType.None : sBlockData.SBlockType;

		GameObject blockObject = blockObject = Object.Instantiate(editorBlockPrefabList.GetBlockPrefab(blockType), blockHolder, true);
		blockObject.transform.localPosition = IndexToLocalPos(x, y);

		var block = blockObject.GetComponent<Block>();
		block.SetData(blockData);

		if (blockType is not SpecialBlockType.None)
		{
			var colorLayer = stageData.sBlockColorTokens.Where(b => b.hex == new Hex(x, y)).Select(x => x.colorLayer).FirstOrDefault();
			block.SetColor(colorLayer);
		}
		
		blocks[y, x] = block;
	}
	
	private Vector3 IndexToLocalPos(int x, int y)
	{
		float nx = x * ColGap;
		float ny = y * RowGap + (x % 2 == 1 ? -RowGap * 0.5f : 0);

		return new Vector3(nx, ny, 0);
	}

	public Vector3 IndexToWorldPos(Hex hex)
	{
		int x = hex.x;
		int y = hex.y;
		return blockHolder.transform.TransformPoint(IndexToLocalPos(x, y));
	}

	public bool IsIndexOutOfRange(Hex hex)
	{
		return hex.x < 0 || hex.y < 0 || hex.x >= Col || hex.y >= Row;
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

	public bool CanNotSetBlock(Hex hex)
	{
		var cellData = stageData.GetCellData(hex.x, hex.y);

		return cellData.cellType != CellType.Basic;
	}
	#region 타겟블록

	public void SetTarget(TargetData targetData, Hex hex,int value)
	{
		if (targetData is null)
		{ 
			GameObject.DestroyImmediate(targets[hex.y, hex.x]);
			targets[hex.y, hex.x] = null;
			return;
		}
		
		var targetType = targetData.TargetObjectType;
	
		if (targetType == TargetObjectType.Block)
		{
			var target_Block_Data = (Target_Block_Data)targetData;
			
			var blockObj = GetTarget(hex);
			
			// 이미 배치된 블록이 없는 위치인데 우클릭(-1) 했으면 아무것도 하지 않음
			if (blockObj == null && value == -1) return;
			
			// 블록이 없다면 새 블록 생성
			if (blockObj == null && value == 1)
			{
				CreateTargetBlock(target_Block_Data, hex);
			}
			else
			{
				var block = blockObj.GetComponent<Block>();
				block.BlockData.HP+= value;
			
				if (blockObj.TryGetComponent<HpSpriteHandler>(out var hpSpriteHandler))
				{
					if (block.BlockData.HP > hpSpriteHandler.MaxHp)
					{
						block.BlockData.HP -= value;
					}
					hpSpriteHandler.Init(block.BlockData.HP);
				}
				
				stageData.AddOrUpdateBlockToken(target_Block_Data, hex, block.BlockData.HP);
				
				// 블록 HP가 0 이하가 되면 제거
				if (block.BlockData.HP <= 0)
				{
					RemoveTargetBlock(hex);
				}
			}
		}
	}

	public GameObject GetTarget(Hex hex) => targets[hex.y, hex.x];
	
	private void CreateTargetBlock(Target_Block_Data targetBlockData, Hex hex)
	{
		// 새 블록 생성
		var blockObject = GameObject.Instantiate(targetBlockData.Prefab, blockHolder, true);
		blockObject.transform.localPosition = IndexToLocalPos(hex.x, hex.y);
        
		var block = blockObject.GetComponent<Block>();

		// BlockData 복제 로직(Reflection 또는 직접 복사 가능)
		// 일단 새 ScriptableObject 생성 후 HP 초기화
		var newBlockData = ScriptableObject.CreateInstance<BlockData>();
		
		var blockData = targetBlockData.BlockData;
		
		// Reflection을 사용해 필드 값 복사
		Util.CopyFields(blockData, newBlockData);
		
		newBlockData.HP = 1;

		block.SetData(newBlockData);
		targets[hex.y, hex.x] = blockObject;
		
		if (blockObject.TryGetComponent<HpSpriteHandler>(out var hpSpriteHandler))
		{
			hpSpriteHandler.Init(block.BlockData.HP);
		}
		
		// StageData에 기록 (타겟 토큰 리스트 업데이트 포함)
		stageData.AddOrUpdateBlockToken(targetBlockData, hex, newBlockData.HP);
	}
	
	private void RemoveTargetBlock(Hex hex)
	{
		var blockObj = targets[hex.y, hex.x];
		if (blockObj != null)
		{
			Object.DestroyImmediate(blockObj);
			targets[hex.y, hex.x] = null;
		}
		stageData.RemoveBlockToken(hex);
	}

    #endregion
	
	
	public void SetBlock((ColorLayer color, BlockData data) blockData, Hex hex)
	{
		GameObject blockObject = null;
		stageData.SetBlockData(hex, blockData.data);

		if (blockData.data is null)
		{
			Object.DestroyImmediate(blocks[hex.y, hex.x]?.gameObject);
			blocks[hex.y, hex.x] = null;
		}
		else
		{
			SpecialBlockType blockType = blockData.data is not SpecialBlockData sBlockData ? SpecialBlockType.None : sBlockData.SBlockType;

			if (blocks[hex.y, hex.x] is null)
			{
				blockObject = Object.Instantiate(editorBlockPrefabList.GetBlockPrefab(blockType), blockHolder, true);
				blockObject.transform.localPosition = IndexToLocalPos(hex.x, hex.y);
			}
			else blockObject = blocks[hex.y, hex.x].gameObject;

			var block = blockObject.GetComponent<Block>();
			block.SetData(blockData.data);

			if (blockType is not SpecialBlockType.None and not SpecialBlockType.Super)
			{
				block.SetColor(blockData.color);
				
				var existingBlock = stageData.sBlockColorTokens.FirstOrDefault(b => b.hex == hex);
				if (existingBlock != null)
				{
					// 이미 존재하면 값 업데이트
					existingBlock.colorLayer = blockData.color;
				}
				else
				{
					stageData.sBlockColorTokens.Add(new StageData.EditorSBlockColorToken()
					{
						hex = hex,
						colorLayer = blockData.color,
					});
				}
			}
		
			blocks[hex.y, hex.x] = block;
		}
	}

	public void SetCell(CellData cellData, Hex hex)
	{
		var data = stageData.GetCellData(hex.x, hex.y);

		if (cellData.cellType == data.cellType)
			return;

		GameObject cellObject = null;

		if ((data.cellType is CellType.Basic or CellType.None && cellData.cellType is CellType.Spawn) ||
		    data.cellType is CellType.Spawn && cellData.cellType is CellType.Basic or CellType.None)
		{
			Object.DestroyImmediate(cells[hex.y, hex.x].gameObject);
			cellObject = Object.Instantiate(cellData.prefab, cellHolder, true);
			cellObject.transform.localPosition = IndexToLocalPos(hex.x, hex.y);
		}
		else cellObject = cells[hex.y, hex.x].gameObject;

		cells[hex.y, hex.x] = cellObject.GetComponent<Cell>();
		stageData.SetCellData(hex, cellData);

		if (cellData.cellType is not CellType.Basic)
			SetBlock((ColorLayer.None, null), hex);
		
		if (cellData.cellType is CellType.None)
			cells[hex.y, hex.x].gameObject.SetActive(false);
	}
}
# endif