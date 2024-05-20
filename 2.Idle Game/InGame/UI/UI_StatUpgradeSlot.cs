using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_StatUpgradeSlot : MonoBehaviour
{
    #region Inspector

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI levelText,statNameText,valueText,upgradeCostText;
    [SerializeField] private Button upgradeBtn;

    #endregion
    
    #region Property

    public Image Icon => icon;
    public Button UpgradeBtn => upgradeBtn;
    public TextMeshProUGUI LevelText => levelText;
    public TextMeshProUGUI StatNameText => statNameText;
    public TextMeshProUGUI ValueText => valueText;
    public TextMeshProUGUI UpgradeCostText => upgradeCostText;

    #endregion
}
