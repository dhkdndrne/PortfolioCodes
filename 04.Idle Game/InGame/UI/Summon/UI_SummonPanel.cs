using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SummonPanel : MonoBehaviour,IPanel
{
	[SerializeField] private GameObject panel;
	
    public void OpenPanel()
    {
	    gameObject.SetActive(true);
	    
	    UIManager.Instance.ClosePanel();
	    UIManager.Instance.OpenPanel(this);
        
        panel.SetActive(true);
    }
    public void ClosePanel()
    {
       panel.SetActive(false);
       gameObject.SetActive(false);
    }
}
