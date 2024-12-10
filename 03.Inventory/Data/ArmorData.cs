using UnityEngine;

[CreateAssetMenu(menuName = "SO/Armor ItemData",fileName = "New Armor ItemData")]
public class ArmorData : EquipmentData
{
   [SerializeField] private int hp;
   [SerializeField] private int defense;

   public int Hp => hp;
   public int Defense => defense;
}
