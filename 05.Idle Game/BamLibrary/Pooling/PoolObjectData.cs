using UnityEngine;

[System.Serializable]
public class PoolObjectData
{
    public const int INITIAL_COUNT = 10;
    public const int MAX_COUNT = 50;

    public string key;
    public GameObject prefab;

    public int initObjectCount = INITIAL_COUNT; // 오브젝트 초기 생성 개수
    public int maxObjectCount = MAX_COUNT; // 큐 내에 보관할 수 있는 오브젝트 최대 개수

    public void SetObjectCount(int initalAmount, int maxAmount)
    {
        initObjectCount = initalAmount == 0 ? INITIAL_COUNT : initalAmount;
        maxObjectCount = maxAmount == 0 ? MAX_COUNT : maxAmount;
    }
    
}
