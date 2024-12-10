using System;
using System.Collections;
using System.Collections.Generic;
using Bam.Puzzle.Target;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI countText;
    
    public void SetUI(TargetToken targetToken ,int count)
    {
        image.sprite = targetToken.sprites[0];
        image.color = ColorManager.Instance.GetColor(targetToken.colorLayer);
        UpdateText(count);
    }

    public void UpdateText(int count)
    {
        countText.text = count.ToString();
    }
}
