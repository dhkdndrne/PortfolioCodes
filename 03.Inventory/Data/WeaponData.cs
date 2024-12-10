using UnityEngine;

[CreateAssetMenu(menuName = "SO/Weapon ItemData",fileName = "New Weapon ItemData")]
public class WeaponData : EquipmentData
{
    [SerializeField] private int damage;
    public int Damage => damage;
}
