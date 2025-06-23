using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Bam.Singleton;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class LowBatterySystem : Singleton<LowBatterySystem>
{
    [SerializeField] private TextMeshProUGUI batteryAmountText;
    
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject mainCanvasObject;
    
    [SerializeField] private Button lowBatteryModeBtn;
    [SerializeField] private Button exitLowBatteryModeBtn;
    public bool IsLowBatterMode { get; private set; }

    private WaitForSeconds waitForSeconds = new WaitForSeconds(600);
    private IEnumerator checkBatteryCo;
    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        OnDemandRendering.renderFrameInterval = 1;
        
        lowBatteryModeBtn.onClick.AddListener(TurnOnLowBatteryMode);
        exitLowBatteryModeBtn.onClick.AddListener(TurnOffLowBatterMode);

        checkBatteryCo = CoCheckBattery();
    }

    private IEnumerator CoCheckBattery()
    {
        while (true)
        {
            batteryAmountText.text = (SystemInfo.batteryLevel * 100).ToString();
            yield return waitForSeconds;
        }
    }
    
    private void TurnOnLowBatteryMode()
    {
        OnDemandRendering.renderFrameInterval = 10;
        IsLowBatterMode = true;
            
        mainCanvasObject.SetActive(false);
        canvas.SetActive(true);

        StartCoroutine(checkBatteryCo);
    }

    private void TurnOffLowBatterMode()
    {
        OnDemandRendering.renderFrameInterval = 1;
        IsLowBatterMode = false;
            
        mainCanvasObject.SetActive(true);
        canvas.SetActive(false);
        
        StopCoroutine(checkBatteryCo);
    }
    
    
    // public int fontSize = 50;
    // public Color color = new Color(1f, 0f, .0f, 1.0f);
    // public float width, height;
    // void OnGUI()
    // {
    //     Rect position = new Rect(width+200, height+200, Screen.width, Screen.height);
    //
    //     float fps = 1.0f / Time.deltaTime;
    //     float ms = Time.deltaTime * 1000.0f;
    //     string text = string.Format("{0:N1} FPS ({1:N1}ms)", fps, ms);
    //
    //     GUIStyle style = new GUIStyle();
    //
    //     style.fontSize = fontSize;
    //     style.normal.textColor = color;
    //
    //     GUI.Label(position, text, style);
    // }
}
