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
	public void CheckBoardIsShuffled(StageData stageData)
	{
		var shapeCheckManager = ShapeCheckManager.Instance;
		List<ColorLayer> unUseableColorLayers = new List<ColorLayer>();

		foreach (var block in board.GetBlockEnumerable())
		{
			unUseableColorLayers.Clear();
			
			var t = stageData.GetBlockData(block.Hex.x, block.Hex.y);
			if (t is not null) continue;
			
			while (shapeCheckManager.CheckAnyShape(block))
			{
				unUseableColorLayers.Add(block.ColorLayer);
				block.SetData(stageData.GetBlockDataExceptColor(unUseableColorLayers));
			}
		}
	}
}