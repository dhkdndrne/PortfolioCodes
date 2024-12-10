using System.Collections;
using System.Collections.Generic;
using Bam.Singleton;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
   [SerializeField] private UI_Currency currencyUI = new UI_Currency();
   public IPanel CurPanel { get; set; }
   
   protected override void Awake()
   {
      base.Awake();
      Initializator.Instance.onSecondInit += currencyUI.Init;
   }

   public void ClosePanel()
   {
      if (CurPanel is null)
         return;
       
      CurPanel.ClosePanel();
      CurPanel = null;
   }

   public void OpenPanel(IPanel panel)
   {
      CurPanel?.ClosePanel();
      CurPanel = panel;
   }
}
