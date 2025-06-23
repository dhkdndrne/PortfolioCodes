using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

public class IngameUIManager : MonoBehaviour,IManger
{
    [SerializeField] private TextMeshProUGUI moveCountText;
    [SerializeField] private TextMeshProUGUI stageText;


    public void InitManager()
    {
        stageText.text = $"Stage\n{Stage.Instance.StageNum}";

        Stage.Instance.MoveCnt.Subscribe(cnt =>
        {
            moveCountText.text = $"Move\n{cnt}";
        }).AddTo(this);
    }
}
