using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private GameObject checkIcon;
    [SerializeField] private TextMeshProUGUI countText;
    public void SetUI(TargetData targetData ,int count)
    {
        image.sprite = targetData.Sprites[0];

        if (targetData.TargetObjectType is TargetObjectType.Block)
        {
            image.color = ColorManager.GetColor(((Target_Block_Data)targetData).ColorLayer);
        }
        
        
        checkIcon.SetActive(false);
        countText.gameObject.SetActive(true);
        UpdateText(count);
    }

    public void UpdateText(int count)
    {
        if (count <= 0)
        {
            checkIcon.SetActive(true);
            countText.gameObject.SetActive(false);
            return;
        }
        
        countText.text = count.ToString();
    }
}
