using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SO/SpecialBlockData",fileName = "New SpecialBlockData")]
public class SpecialBlockData : BlockData
{
   [Header("특수블록")]
   [SerializeField] private SpecialBlockType sBlockType;
   [SerializeField] private string poolingKey;
   [SerializeField] private int condition;
   
   public int Condition => condition;
   public string PoolingKey => poolingKey;
   public SpecialBlockType SBlockType => sBlockType;
}

public struct ReservedSBlockData
{
   public Hex Hex { get; private set; }
   public ColorLayer ColorLayer { get; private set; }
   public SpecialBlockData SpecialBlockData { get; private set; }
   public List<Block> BlockList { get; private set; }

   public ReservedSBlockData(SpecialBlockData specialBlockData,ColorLayer colorLayer, Hex hex, List<Block> list)
   {
      SpecialBlockData = specialBlockData;
      ColorLayer = colorLayer;
      Hex = hex;
      BlockList = list;
   }
}
