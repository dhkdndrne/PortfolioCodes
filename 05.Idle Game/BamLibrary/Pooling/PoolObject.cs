using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PoolObject : MonoBehaviour
{
    public string key;

    /// <summary> 게임오브젝트 복제 </summary>
    public PoolObject Clone()
    {
        GameObject obj = Instantiate(gameObject);
        if (!obj.TryGetComponent(out PoolObject pool))
            pool = obj.AddComponent<PoolObject>();
        obj.SetActive(false);

        return pool;
    }

    /// <summary> 게임오브젝트 활성화 </summary>
    public void Activate()
    {
        gameObject.SetActive(true);
    }

    /// <summary> 게임오브젝트 비활성화 </summary>
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
