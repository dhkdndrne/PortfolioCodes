using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SkillInfoPanel : MonoBehaviour
{
    [SerializeField] private Image skillIcon;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;
    
    [Header("스킬 발동타입,스킬 코스트 회복타입")]
    [SerializeField] private GameObject[] spChargeTypeObjects;
    [SerializeField] private GameObject[] skillActiveTypeObjects;
    

    [Header("지속시간")]
    [SerializeField] private GameObject skillDurationObject;
    [SerializeField] private TextMeshProUGUI durationText;

    public void UpdateUI(Operator op)
    {
        var opSpriteData = ImageManager.Instance.operatorImageDic[op.OperatorID];
        var skill = op.GetSkill();
        
        skillIcon.sprite = opSpriteData.GetSkillIcon(skill.Index);
        skillName.text = skill.SkillName;
        
        OffTypeObjects();
        
        skillActiveTypeObjects[(int)skill.ActiveType].SetActive(true);
        spChargeTypeObjects[(int)skill.SpChargeType].SetActive(true);

        float duration = skill.Duration;
        if (duration <= 0)
        {
            skillDurationObject.SetActive(false);
        }
        else
        {
            skillDurationObject.SetActive(true);
            durationText.text = $"{duration.ToString()}초";
        }
        skillDescription.text = skill.Description;
    }

    private void OffTypeObjects()
    {
        foreach (var obj in spChargeTypeObjects)
        {
            obj.SetActive(false);
        }
        
        foreach (var obj in skillActiveTypeObjects)
        {
            obj.SetActive(false);
        }
    }
}
