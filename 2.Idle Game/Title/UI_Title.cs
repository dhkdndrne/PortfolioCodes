using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_Title : MonoBehaviour
{
  #region MyRegion

  [SerializeField] private Slider loadSlider;
  [SerializeField] private TextMeshProUGUI percentageText;

  #endregion
  
  public void Init(ReactiveProperty<float> loadPercentage)
  {
    loadPercentage.Subscribe(value =>
    {
      loadSlider.value = value;
      percentageText.text = $"{(int)(value * 100)} %";
    });
  }
}
