using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BootManager : MonoBehaviour
{
    [Serializable]
    private class ManagerToken
    {
        public MonoBehaviour mono;
        public int order;
    }

    [SerializeField] private List<ManagerToken> managers;

    private void Awake()
    {
        foreach (var token in managers)
        {
            var m = (IInitializer)token.mono;
            m.OnStartManager();
        }
    }
}
