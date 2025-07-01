using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class State_Clear : State
{
	[SerializeField] SpecialBlockData[] specialBlocks;
	private void Awake()
	{
		OnBeginStream.Subscribe(async _ =>
		{
			Debug.Log("클리어");
			
			if (!Stage.Instance.isClear)
				StartPartyTime().Forget();
			else
			{
				Debug.Log("끝~~");
				await UniTask.Delay(2000);
				SceneManager.LoadScene("LobbyScene");
			}
			
		}).AddTo(this);

		OnEndStream.Subscribe(_ =>
		{

		}).AddTo(this);
	}

	private async UniTaskVoid StartPartyTime()
	{
		int cnt = Stage.Instance.MoveCnt.Value;
		var board = GameManager.Instance.Board;
		List<Block> blocks = new List<Block>();

		//이미 있는 특수블록 넣어줌
		foreach (var block in board.GetBlockEnumerable())
		{
			var sBlock = block.BlockData as SpecialBlockData;
			if (sBlock is not null)
			{
				PopBlockDataManager.Instance.SpecialBlocks.Add((block.Hex, sBlock.SBlockType, block.GetComponent<ISpecialBlockBehaviour>()));
				continue;
			}

			blocks.Add(block);
		}

		while (cnt > 0)
		{
			int randomIndex = Random.Range(0, blocks.Count);
			var randomBlock = blocks[randomIndex];

			blocks.RemoveAt(randomIndex);
			board.RemoveBlock(randomBlock.Hex);

			var newBlock =
				BlockSpawner.Instance.SpawnSpecialBlock(new ReservedSBlockData(specialBlocks[Random.Range(0, specialBlocks.Length)],
					randomBlock.ColorLayer, randomBlock.Hex, null));

			board.SetBlock(randomBlock.Hex, newBlock);
			newBlock.transform.position = board.IndexToWorldPos(newBlock.Hex);

			PopBlockDataManager.Instance.SpecialBlocks.Add((newBlock.Hex,((SpecialBlockData)newBlock.BlockData).SBlockType,newBlock.GetComponent<ISpecialBlockBehaviour>()));
			cnt--;
			Stage.Instance.MoveCnt.Value--;
			await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
		}

		Stage.Instance.isClear = true;
		ChangeState<State_Pop>();
	}
}