using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    #region Field

    private bool isActivated;
    private float loadNum;
    private float targetValue;
    private float middleValue;
    
    private readonly float loadDuration = 1f;
    
    private AsyncOperation loadOperation;
    private UI_Title titleUI;
    
    #endregion

    #region Property

    private ReactiveProperty<float> loadPercentage = new();
    
    #endregion

    private void Awake()
    {
        titleUI = GetComponent<UI_Title>();
        titleUI.Init(loadPercentage);
    }

    private void Start()
    {
        StartLoad().Forget();
    }

    private async UniTask LoadData()
    {
        await DataManager.Instance.Init();
    }

    private async UniTaskVoid StartLoad()
    {
        UniTask percentage = new();
        List<UniTask> loadingTasks = new List<UniTask>();
        
        loadingTasks.Add(UniTask.Defer(LoadData));
        loadingTasks.Add(UniTask.Defer(OnSceneAsync));

        foreach (var task in loadingTasks)
        {
            await task;
            targetValue = ++loadNum / loadingTasks.Count;
            if (!isActivated)
            {
                percentage = UpdateLoadPercentage();
                isActivated = true;
            }
        }
        
        await percentage;
        FadeManager.Instance.FadeAsync(UniTask.Defer(AllowSceneActivation)).Forget();
    }
    
    private async UniTask UpdateLoadPercentage()
    {
        float value = middleValue;
        
        while (true)
        {
            value += Time.deltaTime;
            float loadRatio = value / loadDuration;

            loadPercentage.Value = loadRatio >= 1f ? 1f : loadRatio;

            if (loadPercentage.Value >= targetValue || loadPercentage.Value >= 1f)
            {
                break;
            }

            await UniTask.Yield();
        }

        middleValue = targetValue;
        isActivated = false;
    }
    
    private async UniTask OnSceneAsync()
    {
        loadOperation = SceneManager.LoadSceneAsync("GameScene");
        loadOperation.allowSceneActivation = false;
        await UniTask.Yield();
    }
    
    private async UniTask AllowSceneActivation()
    {
        loadOperation.allowSceneActivation = true;
        await UniTask.Yield();
    }
}
