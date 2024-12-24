using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetPopUp : MonoBehaviour
{
    [SerializeField] private GameObject[] targetObjects;
    private TargetUIToken[] targetTokens;
    private RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        targetTokens = new TargetUIToken[targetObjects.Length];
        for (int i = 0; i < targetObjects.Length; i++)
        {
            targetTokens[i] = new TargetUIToken()
            {
                image = targetObjects[i].GetComponentInChildren<Image>(),
                text = targetObjects[i].GetComponentInChildren<TextMeshProUGUI>()
            };
            targetObjects[i].SetActive(false);
        }

        SetTargetUI();
    }

    public async UniTask MovePopUp()
    {
        gameObject.SetActive(true);
        rectTransform.anchoredPosition = new Vector3(-Screen.width, 0,0);
        
        await rectTransform.DOAnchorPosX(0,0.5f).SetEase(Ease.Linear).ToUniTask();
        await UniTask.Delay(1000);
        await rectTransform.DOAnchorPosX(Screen.width,0.5f).SetEase(Ease.Linear).ToUniTask();
        gameObject.SetActive(false);
        await UniTask.Delay(500);
    }
    
    private void SetTargetUI()
    {
        var stageData = StageManager.stageData;
        var targets = stageData.GetTargetList();

        for (int i = 0; i < targets.Count; i++)
        {
            targetObjects[i].SetActive(true);
			
            targetTokens[i].image.sprite = targets[i].targetData.Sprites[0];

            if (targets[i].targetData.TargetObjectType is TargetObjectType.Block)
            {
                var targetBlockData = (Target_Block_Data)targets[i].targetData;
                targetTokens[i].image.color = ColorManager.GetColor(targetBlockData.ColorLayer);
            }
			
            targetTokens[i].text.text = targets[i].count.ToString();
        }
    }
    
    private class TargetUIToken
    {
        public Image image;
        public TextMeshProUGUI text;
    }
}
