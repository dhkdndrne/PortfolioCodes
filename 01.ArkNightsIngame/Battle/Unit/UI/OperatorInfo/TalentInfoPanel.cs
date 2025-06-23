using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TalentInfoPanel : MonoBehaviour
{
    [SerializeField] private GameObject[] talentObjects;
    [SerializeField] private TextMeshProUGUI[] nameTexts;
    [SerializeField] private TextMeshProUGUI[] descriptionTexts;
    
    public void UpdateUI(IReadOnlyList<TalentHandler> talents)
    {
        foreach (var obj in talentObjects)
        {
            obj.SetActive(false);
        }
        
        int i = 0;
        foreach (var t in talents)
        {
            talentObjects[i].SetActive(true);
            nameTexts[i].text = t.Data.DisplayName;
            descriptionTexts[i].text = t.Data.Description;

            i++;
        }
    }
}
