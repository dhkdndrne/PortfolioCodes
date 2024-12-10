using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopBlockDataManager : ObjectSingleton<PopBlockDataManager>
{
	public HashSet<Block> DropSet { get; private set; } = new HashSet<Block>();
	public HashSet<Block> SwapSet { get; private set; } = new HashSet<Block>();
	public HashSet<Block> PopSet {get;private set;}= new HashSet<Block>();

	public (Hex hex, SpecialBlockType blockType, ISpecialBlockBehaviour behaviour) CombinedSBlock; //특수블록 합성


}