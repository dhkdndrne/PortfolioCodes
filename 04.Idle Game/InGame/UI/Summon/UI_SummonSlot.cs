using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_SummonSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText,expText;
    [SerializeField] private Slider slider;

    [SerializeField] private Button summon_10_Btn, summon_100_Btn;
    
    public void Init(SummonSlot slot)
    {
        slot.Level.Subscribe(value =>
        {
            levelText.text = value.ToString();
        }).AddTo(this);

        slot.Exp.Subscribe(value =>
        {
            expText.text = value == slot.MaxExp.Value ? "Max": $"{value} / {slot.MaxExp.Value}";
            slider.value = (float)value / slot.MaxExp.Value;
        }).AddTo(this);
        
        summon_10_Btn.onClick.AddListener(slot.Summon_10);
        summon_100_Btn.onClick.AddListener(slot.Summon_100);
    }
}
