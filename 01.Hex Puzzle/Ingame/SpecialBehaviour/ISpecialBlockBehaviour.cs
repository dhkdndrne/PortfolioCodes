using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ISpecialBlockBehaviour
{
    public void Execute(Hex hex,SpecialBlockType blockType,HashSet<Block> withinRangeList);
    public UniTask Anim(Block block);
}
