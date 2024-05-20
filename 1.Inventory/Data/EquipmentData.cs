using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentData : ItemData
{
   [SerializeField] private int durability;
   public int Durability => durability;
}
