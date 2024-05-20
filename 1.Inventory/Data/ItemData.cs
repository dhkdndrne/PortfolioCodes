using UnityEngine;

[CreateAssetMenu(menuName = "SO/ItemData",fileName = "New ItemData")]
public class ItemData : ScriptableObject
{
    [SerializeField] private int id;
    [SerializeField] private string itemName;
    [SerializeField] private ItemType itemType;
    [SerializeField] private ItemRank rank;
    [SerializeField] private int sellPrice;
    [SerializeField] private int maxAmount;

    [Multiline]
    [SerializeField] private string description;
    [SerializeField] private Sprite sprite;

    public int ID => id;
    public string Name => itemName;
    public ItemType ItemType => itemType;
    public ItemRank Rank => rank;
    public int SellPrice => sellPrice;
    public int MaxAmount => maxAmount;
    public Sprite Sprite => sprite;
    public string Description => description;
}

public enum ItemType
{
    Equipment = 0,
    Consumable,
    Etc
}
public enum ItemRank
{
    Normal = 0,
    Rare,
    Epic,
    Legend
}
