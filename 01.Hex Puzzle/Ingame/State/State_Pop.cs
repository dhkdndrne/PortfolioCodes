using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class State_Pop : State
{
	[SerializeField] private float mergeMoveSpeed;

	private HashSet<Block> hpSet = new();
	private HashSet<Block> frameVisitSet = new();  //방문 체크한 블록 리스트
	private HashSet<Block> popSet = new();         // 터지는 블록 리스트
	private HashSet<Block> usedSBlockList = new(); // 사용한 아이템 리스트

	private Dictionary<Block, int> hpDic = new(); // hp 감소 시킬 블록 dictionary


	private List<UniTask> blockMergeMoveTasks = new(); // 특수블록생성 위한 블록 움직임 task
	private List<UniTask> taskList = new();            // 여러개의 task를  처리하기위한 리스트

	private readonly string BLOCK_POPFX_KEY = "BlockPop";
	private void Awake()
	{
		OnBeginStream.Subscribe(_ =>
		{
			StartPop().Forget();
		}).AddTo(this);

		OnEndStream.Subscribe(_ =>
		{
			usedSBlockList.Clear();
			blockMergeMoveTasks.Clear();
			hpSet.Clear();
			frameVisitSet.Clear();
			popSet.Clear();
		}).AddTo(this);
	}

	private async UniTaskVoid StartPop()
	{
		if (await PopAllBlock())
		{
			ChangeState<State_Drop>();
			return;
		}

		if (Stage.Instance.CheckFinish())
		{
			var board = GameManager.Instance.Board;

			if (Stage.Instance.isClear && board.HasSpecialBlock())
			{
				PopBlockDataManager.Instance.SpecialBlocks.AddRange(
					board.GetBlockEnumerable()
						.Where(block => block.BlockData is SpecialBlockData) // block을 명확히 정의
						.Select(block => (
							block.Hex,
							((SpecialBlockData)block.BlockData).SBlockType, // 명시적 캐스팅
							block.GetComponent<ISpecialBlockBehaviour>()
						))
				);

				ChangeState<State_Drop>();
			}
			else
			{
				ChangeState<State_Clear>();
			}
		}
		else
		{
			if (Stage.Instance.MoveCnt.Value > 0)
			{
				GameManager.Instance.Board.CheckCanMatch();
				
				ChangeState<State_Input>();
			}
			else
				ChangeState<State_GameOver>();
			
		}
	}

	private async UniTask<bool> PopAllBlock()
	{
		var popBlockDataManager = PopBlockDataManager.Instance;

		var swapSet = popBlockDataManager.SwapSet;
		var dropSet = popBlockDataManager.DropSet;

		List<ReservedSBlockData> itemList = new List<ReservedSBlockData>();
		HashSet<Block> withinRangeList = new();

		var board = GameManager.Instance.Board;
		
		bool isShadowActived = false;

		if (popBlockDataManager.SpecialBlocks.Count > 0)
		{
			board.SetCellShadow(true);
			isShadowActived = true;
		}
		
		foreach (var sBlock in popBlockDataManager.SpecialBlocks)
		{
			//범위 체크
			sBlock.behaviour.Execute(sBlock.hex, sBlock.blockType, withinRangeList);

			//특수블록 애니메이션 실행
			await sBlock.behaviour.Anim(board.GetBlock(sBlock.hex));
			withinRangeList.AddRange(popBlockDataManager.PopSet);

			foreach (var b in withinRangeList)
			{
				if (hpDic.ContainsKey(b))
				{
					hpDic[b]--;
				}
				else hpDic.Add(b, -1);
			}

			withinRangeList.Clear();
		}
		popBlockDataManager.SpecialBlocks.Clear();


		CheckShape(swapSet, hpSet, itemList);
		CheckShape(dropSet, hpSet, itemList);

		swapSet.Clear();
		dropSet.Clear();

		CheckDuplicatedBlock(itemList, out var newItemList);
		frameVisitSet.Clear();
		withinRangeList.Clear();

		bool isPoped = false;
		bool isLoop = true;
		
		while (isLoop)
		{
			taskList.Clear();
			bool isItemLoop = true;
			while (isItemLoop)
			{
				foreach (var v in hpDic)
				{
					frameVisitSet.Add(v.Key);
				}

				DecreaseHp();
				isPoped |= popSet.Count > 0;
				hpDic.Clear();

				//터진 블록 중 특수 블록 있으면 동작
				foreach (var block in popSet)
				{
					if (block.BlockData is not SpecialBlockData) continue;
					if (popBlockDataManager.PopSet.Contains(block)) continue;
					if (!usedSBlockList.Add(block)) continue;

					if (!isShadowActived)
					{
						board.SetCellShadow(true);
						isShadowActived = true;
					}

					var sb = block.GetComponent<ISpecialBlockBehaviour>();
					sb.Execute(block.Hex, SpecialBlockType.None, withinRangeList);
					taskList.Add(sb.Anim(block));

					foreach (var b in withinRangeList)
					{
						if (b == block) continue;
						if (b == null) continue;

						if (hpDic.ContainsKey(b))
						{
							hpDic[b]--;
						}
						else hpDic.Add(b, -1);
					}
				}

				isItemLoop = hpDic.Count > 0;
			}
			
			if (isShadowActived && hpSet.Count > 0)
			{
				HashSet<Block> blockSet = new HashSet<Block>();
				blockSet.AddRange(hpSet);
				taskList.Add(board.ShowTargetHighlightAll(blockSet));
			}
			
			await UniTask.WhenAll(taskList);
			board.SetCellShadow(false);

			await CreateItemMove(newItemList);

			PopBlock();
			await StartBlockPopAnimation();
			isLoop = hpDic.Count > 0;
		}

		foreach (var item in newItemList)
		{
			board.CreateNewItemBlock(item);
		}
		
		popBlockDataManager.PopSet.Clear();
		return isPoped;
	}

	private async UniTask StartBlockPopAnimation()
	{
		taskList.Clear();
		foreach (var block in popSet)
		{
			var obj = ObjectPoolManager.Instance.Spawn(BLOCK_POPFX_KEY);
			obj.transform.position = block.transform.position;

			var fx = obj.GetComponent<SpriteAnimator>();
			taskList.Add(fx.StartAnimation());
		}

		await UniTask.WhenAll(taskList);
	}
	private void CheckShape(HashSet<Block> list, HashSet<Block> hpSet, List<ReservedSBlockData> itemList)
	{
		//리스트의 블록들을 기준으로 모양 체크
		foreach (var block in list)
		{
			ShapeCheckManager.Instance.CheckShapes(block, hpSet, itemList);
		}

		//블록들 hp감소 딕셔너리에 등록
		foreach (var block in hpSet)
		{
			if (!hpDic.TryGetValue(block, out var val))
			{
				hpDic.Add(block, -1);
			}
			else val -= 1;
		}
	}
	private void DecreaseHp()
	{
		foreach (var b in hpDic)
		{
			var block = b.Key;
			block.SetHP(b.Value);
			
			if (block.HP <= 0)
			{
				popSet.Add(block);
			}
			
			if (block.TryGetComponent<HpSpriteHandler>(out var hpSpriteHandler))
			{
				hpSpriteHandler.ChangeSprite(block.HP);
			}
		}
	}
	private void PopBlock()
	{
		var stage = Stage.Instance;
		var board = GameManager.Instance.Board;
		foreach (var block in popSet)
		{
			//주변타일 영향 주는지
			if (block.BlockData.CanAffectOther)
			{
				for (int i = 0; i < (int)HexWay.Length; i++)
				{
					var nHex = Hex.GetHexByWay(block.Hex, (HexWay)i);
					if (board.IsIndexOutOfRange(nHex)) continue;
					if (!board.IsValidIndex(nHex)) continue;

					var nb = board.GetBlock(nHex);

					if (nb == null) continue;
					if (!nb.BlockData.CanAffected) continue;

					if (frameVisitSet.Add(nb))
					{
						if (hpDic.ContainsKey(nb))
							hpDic[nb]--;
						else
							hpDic.Add(nb, -1);
					}
				}
			}

			//블록 위 셀에 영향 주는지
			//var cell = board.GetCell(block.Hex);

			//점수 추가
			stage.Score.Value += 10;
			stage.UpdateBlockTarget(block, -1);
			board.RemoveBlock(block.Hex);
		}
	}

	/// <summary>
	/// 같은블록으로 여러개의 아이템 못만들게 체크
	/// </summary>
	/// <param name="itemList"></param>
	private void CheckDuplicatedBlock(List<ReservedSBlockData> list, out List<ReservedSBlockData> itemList)
	{
		itemList = list.Where(data =>
		{
			if (data.BlockList.Any(b => frameVisitSet.Contains(b))) return false;
			frameVisitSet.AddRange(data.BlockList);
			return true;
		}).ToList();
	}
	private async UniTask CreateItemMove(List<ReservedSBlockData> list)
	{
		var board = GameManager.Instance.Board;
		blockMergeMoveTasks.Clear();
		
		foreach (var data in list)
		{
			if (data.BlockList == null) continue;

			Vector3 targetPos = board.IndexToWorldPos(data.Hex);

			foreach (var block in data.BlockList)
			{
				var task = block.transform.DOMove(targetPos, mergeMoveSpeed).SetEase(Ease.InOutCirc).ToUniTask();
				blockMergeMoveTasks.Add(task);
			}
		}
	
		await UniTask.WhenAll(blockMergeMoveTasks);
	}

}