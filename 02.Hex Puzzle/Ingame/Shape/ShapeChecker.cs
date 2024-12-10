using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class ShapeChecker : MonoBehaviour
{
    [SerializeField] protected SpecialBlockData specialBlockData;
    [SerializeField] protected int minCondition;
    protected List<Block> list = new List<Block>();
    
    public abstract bool CheckShape(Block block,HashSet<Block> popList = null,List<ReservedSBlockData> itemList = null);
}

