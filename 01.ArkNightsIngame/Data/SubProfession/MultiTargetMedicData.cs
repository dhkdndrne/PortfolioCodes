using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/SubProfession/MultiTargetMedic")]
public class MultiTargetMedicData : SubProfessionData,ITargetingTrait
{
    private const int HEAL_TARGET_COUNT = 3;
    
    public override void ApplyTrait(OperatorController op)
    {
      
    }
   
    public IEnumerable<Unit> GetTargets(List<Tile> tilesInAttackRange)
    {
	    // tilesInAttackRange 안에서 치료가 필요한 유닛 뽑기
	    var inRange = tilesInAttackRange
		    .Select(t => t.UnitOnTile)
		    .Where(u => u != null
		                && !u.IsDead()
		                && u.Attribute.HpRatio.Value < 1f)
		    .OrderBy(u => u.Attribute.HpRatio.Value)
		    .ToList();

	    // healCount만큼 리턴
	    return inRange.Take(HEAL_TARGET_COUNT);
    }
}
