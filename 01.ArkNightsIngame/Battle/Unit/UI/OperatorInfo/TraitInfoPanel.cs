using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TraitInfoPanel : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;

    public void UpdateUI(SubProfessionData subProfessionData)
    {
        icon.sprite = subProfessionData.Icon;
        title.text = subProfessionData.Name;
        description.text = subProfessionData.Description;
    }
}
