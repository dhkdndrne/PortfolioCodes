using System.Collections.Generic;
using UnityEngine;
using Bam.Extensions;
public class AttackRangeIndicator : ObjectSingleton<AttackRangeIndicator>
{
	private List<Tile> attackRangeTiles = new List<Tile>();

	public AttackRangeIndicator()
	{
		ClickChecker.Instance.onOperatorClicked += ShowDeployedOperatorAttackRange;
	}

	/// <summary>
	/// 이미 배치된 오퍼레이터 공격범위 보여주기
	/// </summary>
	/// <param name="op"></param>
	private void ShowDeployedOperatorAttackRange(Operator op)
	{
		DisableAttackRange();
		if (op != null)
		{
			attackRangeTiles.AddRange(AttackRangeHandler.GetAttackRangeTiles(op.OnTile,op.GetCurAttackRangeGrid()));
			foreach (var tile in attackRangeTiles)
			{
				tile.SetAttackRangeShaderState(true);
			}
		}
	}

	public void ShowUndeployedOperatorAttackRange(Operator op,Tile onTile,OperatorDirection dir = OperatorDirection.Right)
	{
		DisableAttackRange();
		if (op != null)
		{
			var grid = Extensions.RotateArray(op.GetOriginAttackRangeGrid(), ((int)dir - 1) * 90);
			attackRangeTiles.AddRange(AttackRangeHandler.GetAttackRangeTiles(onTile,grid));
			foreach (var tile in attackRangeTiles)
			{
				tile.SetAttackRangeShaderState(true);
			}
		}
	}
	
	public void DisableAttackRange()
	{
		foreach (var tile in attackRangeTiles)
		{
			tile.SetAttackRangeShaderState(false);
		}
		attackRangeTiles.Clear();
	}
}