using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Bam.Singleton;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    [SerializeField]
    private List<PoolObjectData> poolObjDataList = new ();  //인스펙터에 풀링대상 정보 추가

    // 복제될 오브젝트의 원본 딕셔너리
    private Dictionary<string, PoolObject> dicSample;

    // 풀링 정보 딕셔너리
    private Dictionary<string, PoolObjectData> dicData;

    // 풀 딕셔너리
    private Dictionary<string, Stack<PoolObject>> dicPool;
    
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        int length = poolObjDataList.Count;
        if (length == 0) return;

        // 1. Dictionary 생성
        dicSample = new Dictionary<string, PoolObject>(length);
        dicData = new Dictionary<string, PoolObjectData>(length);
        dicPool = new Dictionary<string, Stack<PoolObject>>(length);

        // 2. Data로부터 새로운 Pool 오브젝트 정보 생성
        //foreach (var data in poolObjDataList)
        //{
        //    Register(data);
        //}
    }

    /// <summary> Pool 데이터로부터 새로운 Pool 오브젝트 정보 등록 </summary>
    private void Register(PoolObjectData data)
    {
        // 중복 키는 등록 불가능
        if (dicPool.ContainsKey(data.key))
        {
            return;
        }

        //부모 오브젝트 생성
        var parentObj = new GameObject();
        parentObj.name = data.key + "Pool";
        parentObj.transform.SetParent(this.transform);
        parentObj.transform.localScale = Vector3.one;
        parentObj.transform.position = Vector3.zero;

        // 1. 샘플 게임오브젝트 생성, PoolObject 컴포넌트 존재 확인
        GameObject sample = Instantiate(data.prefab, parentObj.transform);
        if (!sample.TryGetComponent(out PoolObject po))
        {
            po = sample.AddComponent<PoolObject>();
            po.key = data.key;
        }
        sample.SetActive(false);

        // 2. Pool Dictionary에 풀 생성 + 풀에 미리 오브젝트들 만들어 담아놓기
        Stack<PoolObject> pool = new Stack<PoolObject>(data.maxObjectCount);
        for (int i = 0; i < data.initObjectCount; i++)
        {
            PoolObject clone = po.Clone();
            clone.transform.SetParent(parentObj.transform);

            pool.Push(clone);
        }

        // 3. 딕셔너리에 추가
        dicSample.Add(data.key, po);
        dicData.Add(data.key, data);
        dicPool.Add(data.key, pool);
    }

    /// <summary> 풀에서 꺼내오기 </summary>
    public PoolObject Spawn(string key)
    {
        // 키가 존재하지 않는 경우 null 리턴
        if (!dicPool.TryGetValue(key, out var pool))
        {
            bool isExist = false;
            foreach (var data in poolObjDataList)
            {
                if(data.key.Equals(key))
                {
                    Register(data);
                    pool = dicPool[key];
                    isExist = true;
                    break;
                }                 
            }

            if(!isExist) return null;
        }

        PoolObject po;

        // 1. 풀에 재고가 있는 경우 : 꺼내오기
        if (pool.Count > 0)
        {
            po = pool.Pop();
        }
        // 2. 재고가 없는 경우 샘플로부터 복제
        else
        {
            po = dicSample[key].Clone();
        }

        po.Activate();
        return po;
    }

    /// <summary> 풀에 집어넣기 </summary>
    public void Despawn(PoolObject po)
    {
        // 키가 존재하지 않는 경우 종료
        if (!dicPool.TryGetValue(po.key, out var pool))
        {
            return;
        }

        string key = po.key;

        // 1. 풀에 넣을 수 있는 경우 : 풀에 넣기
        if (pool.Count < dicData[key].maxObjectCount)
        {
            pool.Push(po);
            po.Deactivate();
        }
        // 2. 풀의 한도가 가득 찬 경우 : 파괴하기
        else
        {
            Destroy(po.gameObject);
        }
    }
 
}
