using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BoardShuffleSystem
{
	private Board board;

	public BoardShuffleSystem(Board board)
	{
		this.board = board;
	}

	/// <summary>
	/// board에 매칭이 되는 블록이 있는지 체크
	/// 만약 매칭되는 블록이 있으면 다른 블록으로 교체 후 다시 체크
	/// </summary>
	/// <param name="stageData"></param>
	public void ValidateStageShuffle()
	{
		var shapeCheckManager = ShapeCheckManager.Instance;
		List<ColorLayer> unUseableColorLayers = new List<ColorLayer>();

		foreach (var block in board.GetBlockEnumerable())
		{
			unUseableColorLayers.Clear();
			
			var t = StageManager.stageData.GetBlockData(block.Hex.x, block.Hex.y);
			if (t is not null) continue;
			
			while (shapeCheckManager.CheckAnyShape(block))
			{
				unUseableColorLayers.Add(block.ColorLayer);
				block.SetData(StageManager.stageData.GetBlockDataExceptColor(unUseableColorLayers));
			}
		}
	}

	public void CheckMatchingBlocks()
	{
		var shapeCheckManager = ShapeCheckManager.Instance;
		List<ColorLayer> unUseableColorLayers = new List<ColorLayer>();

		foreach (var block in board.GetBlockEnumerable())
		{
			unUseableColorLayers.Clear();
			
			while (shapeCheckManager.CheckAnyShape(block))
			{
				unUseableColorLayers.Add(block.ColorLayer);
				block.SetData(StageManager.stageData.GetBlockDataExceptColor(unUseableColorLayers));
				Debug.Log("블록 데이터 변경");
			}
		}
		
	}
}