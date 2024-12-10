using System.Collections.Generic;
using Bam.Singleton;
using UnityEngine;

public class ShapeCheckManager : Singleton<ShapeCheckManager>,IManger
{
   [SerializeField] private ShapeChecker[] shapeCheckers;
   
    /// <summary>
    /// 스왑된 블록 팝 가능한지 체크
    /// </summary>
    /// <param name="b1"></param>
    /// <param name="b2"></param>
    /// <returns></returns>
    public bool CanPopSwappedBlocks(Block b1, Block b2)
    {
        if (CheckAnyShape(b1)) return true;
        if (CheckAnyShape(b2)) return true;

        return false;
    }

    /// <summary>
    /// pop되는게 하나라도 있는지 체크
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    public bool CheckAnyShape(Block block)
    {
        foreach (var checker in shapeCheckers)
        {
            if (checker.CheckShape(block)) 
                return true;
        }
        return false;
    }

    public void CheckShapes(Block block,HashSet<Block> popList,List<ReservedSBlockData> reservedItemDataList)
    {
        //bool canPop = false;
        foreach (var checker in shapeCheckers)
        {
            bool check = checker.CheckShape(block, popList,reservedItemDataList);
            //canPop |= check;
        }
        //return canPop;
    }
    public void InitManager()
    {
        shapeCheckers = GetComponentsInChildren<ShapeChecker>();
    }
}
