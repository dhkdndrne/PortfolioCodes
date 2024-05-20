using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_OffLineReward : MonoBehaviour
{
   [SerializeField] private Sprite[] goldSprites;
   [SerializeField] private Image goldImgage;
   [SerializeField] private TextMeshProUGUI text;

   public void ShowReward(int hour,string gold)
   {
       
      Sprite goldSprite = hour switch
      {
         >= 48 => goldSprites[5],
         >= 24 => goldSprites[4],
         >= 12 => goldSprites[3],
         >= 10 => goldSprites[2],
         >= 2 => goldSprites[1],
         _ => goldSprites[0]
      };
   
      goldImgage.sprite = goldSprite;
      text.text = gold;

   }
}
