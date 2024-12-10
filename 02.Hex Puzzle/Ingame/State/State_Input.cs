using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class State_Input : State
{
	[SerializeField] private float dragDist;
	[SerializeField] private InputAction clickAction;
	[SerializeField] private InputAction clickPosition;

	private bool isInputEnable;

	private Vector2 beg;
	private Vector2 end;
	private Board board;

	private void Awake()
	{
		clickAction.Enable();
		clickPosition.Enable();

		OnBeginStream.Subscribe(_ =>
		{
			isInputEnable = true;

			if (board == null)
				board = GameManager.Instance.Board;

		}).AddTo(this);

		OnUpdateStream.Subscribe(_ =>
		{
			if (!isInputEnable)
				return;

			var pressedThisFrame = clickAction.WasPressedThisFrame();
			var releasedThisFrame = clickAction.WasReleasedThisFrame();

			var clickPos = clickPosition.ReadValue<Vector2>();
			var worldPos = Camera.main.ScreenToWorldPoint(clickPos);

			if (pressedThisFrame)
			{
				beg = worldPos;
			}
			if (releasedThisFrame)
			{
				end = worldPos;
				// 드래그 끝
				Vector2 posDelta = end - beg;

				if (posDelta.sqrMagnitude > dragDist)
				{
					Block sb = null;
					Block tb = null;

					if (TryGetBlocksFromDrag(ref sb, ref tb) && sb.BlockData.CanSwap && tb.BlockData.CanSwap)
					{
						if (Stage.Instance.MoveCnt.Value > 0)
						{
							isInputEnable = false;
							SwipeBlock(sb, tb).Forget();
						}
						// 드래그 실패
						else
						{
							beg = end = Vector2.zero;
						}
					}
					// 클릭
					else
					{
						// todo 아이템 사용
						// 
					}
				}
			}

		}).AddTo(this);
	}

	private bool TryGetBlocksFromDrag(ref Block sb, ref Block tb)
	{
		float minDist = float.MaxValue;
		Hex sHex = board.WorldPosToHex(beg);

		if (board.IsIndexOutOfRange(sHex)) return false;
		sb = board.GetBlock(sHex);

		if (sb is null) return false;
		
		for (int i = 0; i < 6; i++)
		{
			var tHex = Hex.GetHexByWay(sHex, (HexWay)i);

			if (board.IsIndexOutOfRange(tHex)) continue;
			if (!board.IsValidIndex(tHex)) continue;

			var block = board.GetBlock(tHex);

			var wpos = (Vector2)board.IndexToWorldPos(tHex);
			float dist = (end - wpos).sqrMagnitude;

			// 가까운 거리
			if (dist < minDist)
			{
				minDist = dist;
				tb = block;
			}
		}

		return tb != null;
	}

	private async UniTaskVoid SwipeBlock(Block sb, Block tb)
	{
		var sbPos = sb.transform.position;
		var tbPos = tb.transform.position;

		UniTask t1 = default;
		UniTask t2 = default;

		CheckSpecialBlock(sb, tb, out var sSpecialData, out var tSpecialData);
		bool areBothSpecial = tSpecialData && sSpecialData;
		
		//둘다 특수블록일때는 선택블록이 타겟쪽으로 이동하도록 or 슈퍼블록 있을경우 
		t1 = sb.transform.DOMove(tbPos, .25f).SetEase(Ease.InOutCirc).ToUniTask();
		if (areBothSpecial)
		{
			await t1;
		}
		else // 둘다 스왑
		{
			t2 = tb.transform.DOMove(sbPos, .25f).SetEase(Ease.InOutCirc).ToUniTask();
			await UniTask.WhenAll(t1, t2);
		}

		board.ExchangeBlockInfo(sb, tb);
		var popBlockDataManager = PopBlockDataManager.Instance;
		
		if (areBothSpecial)
		{
			popBlockDataManager.SwapSet.Clear();
			SpecialBlockCombiner.Instance.Combine(sb, tb, sSpecialData.SBlockType, tSpecialData.SBlockType);
			//Stage.Instance.MoveCnt.Value--;
			ChangeState<State_Pop>();
			return;
		}
		
		if (sSpecialData?.SBlockType is SpecialBlockType.Super)
		{
			popBlockDataManager.CombinedSBlock = (sb.Hex,SpecialBlockType.Super, sb.GetComponent<ISpecialBlockBehaviour>());
			popBlockDataManager.SwapSet.Add(tb);
			popBlockDataManager.PopSet.Add(sb);
			ChangeState<State_Pop>();
			return;
		}
		else if (tSpecialData?.SBlockType is SpecialBlockType.Super)
		{
			popBlockDataManager.CombinedSBlock = ((tb.Hex,SpecialBlockType.Super, tb.GetComponent<ISpecialBlockBehaviour>()));
			popBlockDataManager.SwapSet.Add(sb);
			popBlockDataManager.PopSet.Add(tb);
			ChangeState<State_Pop>();
			return;
		}

		// //매칭 확인
		bool canPop = ShapeCheckManager.Instance.CanPopSwappedBlocks(sb, tb);
		if (canPop)
		{
			popBlockDataManager.SwapSet.Add(sb);
			popBlockDataManager.SwapSet.Add(tb);
			//Stage.Instance.MoveCnt.Value--;
			ChangeState<State_Pop>();
		}
		else
		{
			//재스왑
			t1 = sb.transform.DOMove(sbPos, .45f).SetEase(Ease.InOutCirc).ToUniTask();
			t2 = tb.transform.DOMove(tbPos, .45f).SetEase(Ease.InOutCirc).ToUniTask();

			await UniTask.WhenAll(t1, t2);
			board.ExchangeBlockInfo(sb, tb);
			isInputEnable = true;
		}
	}

	/// <summary>
	/// 5개 조합 특수블록 있는지 체크
	/// </summary>
	/// <param name="sb"></param>
	/// <param name="tb"></param>
	/// <param name="sSpecialData"></param>
	/// <param name="tSpecialData"></param>
	/// <returns></returns>
	private void CheckSpecialBlock(Block sb, Block tb, out SpecialBlockData sSpecialData, out SpecialBlockData tSpecialData)
	{
		tSpecialData = tb.BlockData as SpecialBlockData;
		sSpecialData = sb.BlockData as SpecialBlockData;

		//bool isTbSpecial = tSpecialData?.SBlockType == SpecialBlockType.Super;
		//bool isTbSpecial = tSpecialData?.SBlockType == SpecialBlockType.Super;

		bool isTbSpecial = tSpecialData != null;
		bool isSbSpecial = sSpecialData != null;
	}
}