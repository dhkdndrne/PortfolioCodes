using TMPro;
using UniRx;
using UnityEngine;

[System.Serializable]
public class UI_Currency
{
    [SerializeField] private TextMeshProUGUI goldText;

    public void Init()
    {
        //플레이어 골드 업데이트 될때 갱신
        Player.Instance.Currency.gold.Subscribe(value =>
        {
            goldText.text = value.TranslateNumber();
        });
        
        //첫 초기화때 한번 갱신
        goldText.text = Player.Instance.Currency.gold.Value.TranslateNumber();
    }
}
