using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [SerializeField] private Image icon;

    public void UpdateUI(Item item)
    {
        nameText.text = item.Data.Name;
        typeText.text = ConvertTypeToString(item.Data.ItemType);

        rankText.text = $"등급 : {ConvertRankToString(item.Data.Rank)}";
        amountText.text = $"소지 개수 : {item.Amount}";
        priceText.text = $"판매 가격 : {item.Data.SellPrice}";
        descriptionText.text = item.Data.Description;

        icon.sprite = item.Data.Sprite;
    }

    private string ConvertTypeToString(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Equipment => "장비 아이템",
            ItemType.Consumable => "소비 아이템",
            ItemType.Etc => "기타 아이템"
        };
    }

    private string ConvertRankToString(ItemRank rank)
    {
        return rank switch
        {
            ItemRank.Normal => "<color=#FFFFFF>노말</color>",
            ItemRank.Rare => "<color=#78EF67>레어</color>",
            ItemRank.Epic => "<color=#E556BA>에픽</color>",
            ItemRank.Legend => "<color=#DB2941>전설</color>",
        };
    }
}
