using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/ItemList",fileName = "new ItemSo List")]
public class ItemSoList : ScriptableObject
{
    [SerializeField] private List<ItemSo> list = new List<ItemSo>();
    public List<ItemSo> List => list;
    
}
