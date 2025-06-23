using System;
using System.Collections;
using System.Collections.Generic;
using Bam.Singleton;
using UnityEngine;

public class PopUpManager : Singleton<PopUpManager>
{
   [SerializeField] private PopUP stagePopUp;
   
   private Stack<PopUP> popUpStack = new Stack<PopUP>();
   
   public Action OnPopUpClosed;

   private void Start()
   {
      OnPopUpClosed += PopStack;
   }


   public void OpenStageInfoPanel()
   {
      popUpStack.Push(stagePopUp);
      stagePopUp.Open();
   }

   private void PopStack()
   {
      if(popUpStack.Count == 0) 
         return;
      
      popUpStack.Pop();
   }
   
}
