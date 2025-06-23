using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NightingaleTalent1Handler : TalentHandler
{
	private Operator owner;
	private float buffValue;
	private HashSet<Unit> inRangeOperators = new HashSet<Unit>();
	private Buff_MagicResist buff;
	
	public override void Initialize(Operator op)
	{
		owner = op;
		buffValue = data.Effects[0].type == AttributeType.MagicResistance ? data.Effects[0].value : 0;
		buff = new Buff_MagicResist(-1, false, buffValue);
		
		op.onDeploy += () =>
		{
			foreach (var tile in owner.TilesInAttackRange)
			{
				var unit = tile.UnitOnTile;
				if (unit is null) continue;
				
				unit.AddBuff(buff);
				inRangeOperators.Add(unit);
			}
		};
		//나이팅게일 죽으면 공격범위안 오퍼레이터 버프 삭제
		op.OnDeath += () =>
		{
			foreach (var oper in inRangeOperators)
			{
				oper.RemoveBuff(buff);
			}
			inRangeOperators.Clear();
		};
		
		GameManager.Instance.OperatorManager.OnOperatorDeployed += oper =>
		{
			if (owner.OnTile is null)
				return;

			foreach (var tile in owner.TilesInAttackRange)
			{
				var unit = tile.UnitOnTile;
				if(unit is null) continue;

				if (inRangeOperators.Add(unit))
				{
					oper.AddBuff(buff);
				}
			}
		};
	}
}