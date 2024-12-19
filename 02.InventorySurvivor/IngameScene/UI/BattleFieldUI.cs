using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BattleFieldUI : MonoBehaviour
{
   [SerializeField] private Slider hpSlider;
   [SerializeField] private Slider expSlider;
   [SerializeField] private TextMeshProUGUI timerText;
   [SerializeField] private TextMeshProUGUI stageText;

   private void Awake()
   {
      PlayerData.Instance.GetAbilityProperty(AbilityType.Hp).Subscribe(hp =>
      {
         hpSlider.value = (float)hp / PlayerData.Instance.GetAbilityProperty(AbilityType.MaxHp).Value;
      }).AddTo(this);

      PlayerData.Instance.Exp.Subscribe(exp =>
      {
         expSlider.value = exp / (float)PlayerData.Instance.MaxExp;
      }).AddTo(this);
   }
}
