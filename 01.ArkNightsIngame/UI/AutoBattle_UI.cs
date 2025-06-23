using UnityEngine;
using UnityEngine.UI;

public class AutoBattle_UI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button change_ManualModeButton;
    
    [Header("경고창")]
    [SerializeField] private GameObject cautionPanel;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button cancelButton;

    private void Start()
    {
        AutoBattleManager.Instance.IsReplayMode.Subscribe(val =>
        {
            panel.SetActive(val);
        });
        
        change_ManualModeButton.onClick.AddListener(() =>
        {
            TimeManager.Instance.Stop();
            cautionPanel.SetActive(true);
        });
        
        cancelButton.onClick.AddListener(() =>
        {
            TimeManager.Instance.Resume();
            cautionPanel.SetActive(false);
        });
        
        acceptButton.onClick.AddListener(() =>
        {
            TimeManager.Instance.Resume();
            AutoBattleManager.Instance.IsReplayMode.Value = false;
            panel.SetActive(false);
            cautionPanel.SetActive(false);
        });
    }
}
