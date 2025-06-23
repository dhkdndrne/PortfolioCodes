using System.Collections.Generic;

public interface ITargetingTrait
{
	IEnumerable<Unit> GetTargets(List<Tile> tilesInAttackRange);
}
