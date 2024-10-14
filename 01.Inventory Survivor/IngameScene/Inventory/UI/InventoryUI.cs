using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform statPanel;
    [SerializeField] private TextMeshProUGUI unlockSlotCntText;
    [SerializeField] private Button rerollButton;
    [SerializeField] private TextMeshProUGUI rerollCostText;
    
    private StatText[] statTexts;

    private void Awake()
    {
        statTexts = statPanel.GetComponentsInChildren<StatText>();
        var abilitys = Enum.GetValues(typeof(AbilityType));
        
        for (int i = 0; i < statTexts.Length; i++)
        {
            statTexts[i].Init((AbilityType)abilitys.GetValue(i));
        }

        Inventory.Instance.UnLockCnt.Subscribe(val =>
        {
            unlockSlotCntText.text = val > 0 ? $"{val} 칸 선택하세요." : "";
        }).AddTo(this);
        
        rerollButton.onClick.AddListener(() =>
        {
            Inventory.Instance.ReollUnlockSlots();
            rerollCostText.text = $"다시 굴리기 <color=#FFEA00>{Inventory.Instance.rerollCost}g</color>";
        });
    }
}
