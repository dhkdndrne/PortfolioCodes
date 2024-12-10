using UnityEngine;

[CreateAssetMenu(menuName = "SO/CellData",fileName = "New CellData")]
public class CellData : ScriptableObject
{
    public CellType cellType;
    public Sprite sprite;
    public GameObject prefab;
    // [Header("아이템이 셀 안에서 이동할 수 있는지 여부")]
    // public bool canMoveIn;
    // [Header("아이템이 셀 밖으로 떨어질 수 있는지 여부")]
    // public bool canMoveOut;
    // [Header("아이템에 의해 파괴될 수 없는지 여부")]
    // public bool cannotBeDestroy;
    // [Header("아이템 위를 덮을 수 있는지 여부")]
    // public bool aboveItem;
}
