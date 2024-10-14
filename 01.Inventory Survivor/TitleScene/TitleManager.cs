using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{

    private void Start()
    {
        InitData().Forget();
    }

    private async UniTaskVoid InitData()
    {
        await DataManager.Instance.LoadData();
        SceneManager.LoadSceneAsync(1);
    }
    
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.A))
    //     {
    //         SceneManager.LoadSceneAsync(1);
    //     }
    // }
}
