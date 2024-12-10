using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SlashSlashBehaviour : MonoBehaviour,ISpecialBlockBehaviour
{
    private HashSet<Block> hashSet;
    public void Execute(Hex hex, HashSet<Block> withinRangeList)
    {
        ShapeCheckUtil.Search(hex, HexWay.LeftDown, withinRangeList);
        ShapeCheckUtil.Search(hex, HexWay.RightUp, withinRangeList);
        
        ShapeCheckUtil.Search(hex, HexWay.RightDown, withinRangeList);
        ShapeCheckUtil.Search(hex, HexWay.LeftUp, withinRangeList);
        
        hashSet = withinRangeList;
    }
    
    public async UniTask Anim(Block block)
    {
	    await GameManager.Instance.Board.ShowTargetHighlightAll(hashSet);
    }
}
