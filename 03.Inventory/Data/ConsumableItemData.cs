using UnityEngine;
[CreateAssetMenu(menuName = "SO/Consumable ItemData",fileName = "New Consumable ItemData")]
public class ConsumableItemData : ItemData
{
    [SerializeField] private float value;
    public float Value => value;
}
