using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TargetUIController : MonoBehaviour
{
	[SerializeField] private TargetUI[] targets; // 고정된 TargetUI 배열

	private Dictionary<Sprite, TargetUI> targetDictionary = new Dictionary<Sprite, TargetUI>();

	private void Start()
	{
		// 모든 TargetUI를 비활성화
		foreach (var t in targets)
		{
			t.gameObject.SetActive(false);
		}

		// 게임 로드 후 타겟 초기화
		WaitForTarget().Forget();
	}

	private async UniTaskVoid WaitForTarget()
	{
		await UniTask.WaitUntil(() => GameManager.Instance.IsLoaded);
		SetTarget();
		SubscribeToEvents();
	}

	private void SetTarget()
	{
		var stageData = Stage.Instance.StageData;
		var targetList = stageData.GetTargetList();

		for (int i = 0; i < targetList.Count; i++)
		{
			var targetData = targetList[i].targetData;
			targets[i].gameObject.SetActive(true); // 배열의 해당 인덱스 활성화
			targets[i].SetUI(targetData, targetList[i].count);

			// Dictionary에 매핑
			targetDictionary[targetData.GetSprite()] = targets[i];
		}
	}

	private void SubscribeToEvents()
	{
		// Stage의 타겟 업데이트 이벤트 구독
		Stage.Instance.OnTargetUpdated += UpdateTargetUI;
	}

	private void OnDestroy()
	{
		// 이벤트 구독 해제
		if (Stage.Instance != null)
		{
			Stage.Instance.OnTargetUpdated -= UpdateTargetUI;
		}
	}

	private void UpdateTargetUI(Sprite sprite, int count)
	{
		if (targetDictionary.TryGetValue(sprite, out var targetUI))
		{
			targetUI.UpdateText(count);
		}
	}
}