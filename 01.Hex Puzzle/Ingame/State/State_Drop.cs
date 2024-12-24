using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class State_Drop : State
{
	/*
	 * 1. 스폰에 생성 가능시 생성
	 * 2. 하나씩 전부 드랍 가능 확인 (아래서부터)
	 *  - 좌우로 내려오는 블럭은 목표지 위 라인에 블럭이 있으면 패스한다.
	 * 3. 한 프레임에 한 칸씩 드랍
	 * 4. 모든 드랍 완료시 팝 모드로 (모든 블럭 드랍 애니메이션 종료, 모든 블럭 생성 종료)
	 *
	 * 애니메이션 딜레이만큼 주기 반복
	 *
	 * */

	private readonly IReadOnlyList<HexWay> dropDownWay = new HexWay[]
	{
		HexWay.Down,
		HexWay.LeftDown,
		HexWay.RightDown
	};

	[SerializeField] private float maxDropDuration;
	[SerializeField] private float acceleration;
	private float dropDuration;

	private HashSet<Block> dropSet = new();
	private bool isDrop;
	private void Start()
	{
		OnBeginStream.Subscribe(_ =>
		{
			dropDuration = maxDropDuration;
			DropCycle().Forget();
		}).AddTo(this);
	}

	private async UniTaskVoid DropCycle()
	{
		do
		{
			isDrop = false;
			DropAllBlocks();
			CreateNewBlock();
			
			await UniTask.Delay(TimeSpan.FromSeconds(dropDuration));
		} while (isDrop);

		//움직인 블록들 체크할 hashSet에 넣어주기
		foreach (Block b in dropSet)
			PopBlockDataManager.Instance.DropSet.Add(b);

		dropSet.Clear();
		
		// 팝 모드로
		ChangeState<State_Pop>();
	}

	private void DropAllBlocks()
	{
		var board = GameManager.Instance.Board;
	
		for (int y = 0; y < board.Row; y++)
		{
			for (int x = 0; x < board.Col; x++)
			{
				if (!board.IsValidIndex(x, y)) continue;

				var block = board.GetBlock(x, y);
				if (block == null) continue;

				isDrop |= DropBlock(block, board);
			}
		}
	}

	private bool DropBlock(Block block, Board board)
	{
		if (!block.BlockData.CanDrop) return false;

		Hex hex = block.Hex;
		foreach (var way in dropDownWay)
		{
			Hex nHex = Hex.GetHexByWay(hex, way);

			if (board.IsIndexOutOfRange(nHex)) continue;
			if (!board.IsValidIndex(nHex)) continue;
			if (board.GetBlock(nHex) != null) continue;

			if (way is HexWay.LeftDown or HexWay.RightDown)
			{
				bool canDrop = true;
				int ny = nHex.y;
				while (true)
				{
					ny += 1;
					var tempHex = new Hex(nHex.x, ny);
					if (board.IsIndexOutOfRange(tempHex)) break;
					if (!board.IsValidIndex(tempHex)) break;

					// 스폰 셀 여부 확인
					if (board.SpawnCellList.Contains(board.GetCell(tempHex)))
					{
						canDrop = false;
						break;
					}
					// 위에 있는 블록의 CanDrop 여부 확인
					var upBlock = board.GetBlock(tempHex);
					if (upBlock != null)
					{
						canDrop = !upBlock.BlockData.CanDrop;
						break;
					}
				}

				if (!canDrop)
					continue;
			}

			// 애니메이션
			Vector2 endPos = board.IndexToWorldPos(nHex);
			block.transform.DOMove(endPos,dropDuration).SetEase(Ease.Linear);
			
			// 블록 등록 및 hex설정 
			board.SetBlock(hex, null);
			board.SetBlock(nHex, block);

			// 떨어지는 블록 set에 추가
			dropSet.Add(block);
			return true;
		}
		return false;
	}

	private void CreateNewBlock()
	{
		var board = GameManager.Instance.Board;

		foreach (var cell in board.SpawnCellList)
		{
			var hex = cell.Hex;
			var nHex = new Hex(hex.x, hex.y - 1);
			
			if (board.IsIndexOutOfRange(nHex)) continue;
			if (!board.IsValidIndex(nHex)) continue;

			var downBlock = board.GetBlock(nHex);
			if (downBlock != null) continue;

			board.CreateNewBlock(hex.x, hex.y);
			var block = board.GetBlock(hex);
			dropDuration = Mathf.Clamp(dropDuration - acceleration, 0.1f, maxDropDuration);
			isDrop |= DropBlock(block, board);
		}
	}
}