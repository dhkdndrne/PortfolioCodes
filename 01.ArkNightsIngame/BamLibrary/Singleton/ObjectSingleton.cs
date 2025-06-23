using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// static으로 설정 할 필요까진 없고, Monobehaviour로 사용할 필요가 없는 클래스에 사용
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ObjectSingleton<T> where T : class, new()
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance is null)
            {
                instance = new T();
            }

            return instance;
        }
    }
}
