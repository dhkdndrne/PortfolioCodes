using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SummonSystem : MonoBehaviour
{
	[SerializeField] private UI_SummonSlot[] summonSlotUIArr;
	[SerializeField] private UI_SummonResult summonResult;

	private Dictionary<ItemType, SummonSlot> summonSlotDic;

	private Action<int> Summon_10_Acion;
	private Action<int> Summon_100_Acion;

	private Dictionary<int, List<(int rank, float rate)>> summonRateDic;

	private List<int> summonList = new List<int>();
	private void Awake()
	{
		summonSlotDic = new Dictionary<ItemType, SummonSlot>();
		summonRateDic = new Dictionary<int, List<(int, float)>>();
		Init();
	}

	private void Init()
	{
		var summonRateData = DataManager.Instance.GetSummonData;
		for (int i = 1; i <= Define.MAX_SUMMON_LEVEL; i++)
		{
			var data = summonRateData[i.ToString()];

			var list = new List<(int, float)>();

			foreach (var value in data)
				list.Add((Convert.ToInt32(value.Item1), Convert.ToSingle(value.Item2)));

			summonRateDic.Add(i, list);
		}

		Summon_10_Acion += type => Summon(type,10);
		Summon_100_Acion += type => Summon(type,100);

		foreach (ItemType enumType in Enum.GetValues(typeof(ItemType)))
		{
			if(enumType == ItemType.Character)
				continue;
			
			var slot = new SummonSlot();
			int index = (int)enumType;
			var data = DataManager.Instance.PlayerData.summonDatas[index];

			slot.Init(data.level, data.exp, data.maxExp, index, Summon_10_Acion, Summon_100_Acion);

			summonSlotDic.Add(enumType, slot);
			summonSlotUIArr[index].Init(slot);
		}
	}
    
	private void Summon(int type,int amount)
	{
		summonList.Clear();
		
		ItemType itemType = (ItemType)type;
		int slotLevel = summonSlotDic[itemType].Level.Value;
        
		//뽑을수 있는 등급 (확률 0 보다 클때) 리스트에 저장
		List<(int rank, float rate)> enableRankList = new List<(int rank, float rate)>();
		for (int i = 0; i < summonRateDic[slotLevel].Count; i++)
		{
			if (summonRateDic[slotLevel][i].rate > 0)
				enableRankList.Add((summonRateDic[slotLevel][i].rank, summonRateDic[slotLevel][i].rate));
		}

		//뽑힌 아이템들 저장할 리스트
		Dictionary<int, (int, int)> pickedItemDic = new Dictionary<int, (int, int)>();
		List<(int,int)> resultList = new List<(int,int)>(); //뽑기 결과용 리스트
		
		for (int i = 0; i < amount; i++)
		{
			float random = Random.Range(0.0001f, 1f);
			float accumulatedRate = 0f; // 누적 확률을 추적

			foreach (var item in enableRankList)
			{
				accumulatedRate += item.rate * 0.01f;

				if (random <= accumulatedRate)
				{
					ItemBase baseItem = ItemManager.Instance.GetRandomItem(itemType, item.rank);
					resultList.Add((baseItem.ID,(int)baseItem.Rank));
					
					if (!pickedItemDic.TryGetValue(baseItem.ID, out var value))
					{
						pickedItemDic.Add(baseItem.ID,(item.rank,1));
					}
					else
					{
						value.Item2++;
						pickedItemDic[baseItem.ID] = value;
					}
					break;
				}
			}
		}
		//데이터 갱신 후 저장
		ItemManager.Instance.RefreshItems(itemType,pickedItemDic);
		DataManager.Instance.SaveData();
		
		//연출
		summonResult.ShowResult(itemType,resultList).Forget();
	}
}