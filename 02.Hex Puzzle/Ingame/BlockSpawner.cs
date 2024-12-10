using System.Linq;
using UnityEngine;
public class BlockSpawner : ObjectSingleton<BlockSpawner>
{
	private readonly string BLOCK_POOLING_KEY = "Block";
	private StageData stageData;

	public BlockSpawner()
	{
		stageData = Stage.Instance.StageData;
	}
	
	public Block SpawnRandomBlock()
	{
		return CreateBlock(stageData.GetRandomBlockData());
	}

	public Block SpawnBlock(BlockData blockData,Hex hex)
	{
		return blockData is SpecialBlockData specialBlockData 
			? SpawnSpecialBlock(specialBlockData,hex) 
			: CreateBlock(blockData);
	}

	private Block CreateBlock(BlockData blockData)
	{
		var blockObject = ObjectPoolManager.Instance.Spawn(BLOCK_POOLING_KEY);
		var block = blockObject.GetComponent<Block>();
		block.SetData(blockData);
		return block;
	}
	
	public Block SpawnSpecialBlock(ReservedSBlockData data)
	{
		var blockObject = ObjectPoolManager.Instance.Spawn(data.SpecialBlockData.PoolingKey);
		
		var block = blockObject.GetComponent<Block>();
		block.SetData(data.SpecialBlockData);
		block.SetColor(data.ColorLayer);
		return block;
	}

	private Block SpawnSpecialBlock(SpecialBlockData blockData,Hex hex)
	{
		var blockObject = ObjectPoolManager.Instance.Spawn(blockData.PoolingKey);
		
		var block = blockObject.GetComponent<Block>();
		block.SetData(blockData);

		var color = stageData.sBlockColorTokens.Where(t => t.hex == hex).Select(x => x.colorLayer).FirstOrDefault();
		Debug.Log($"{hex.y} / {hex.x} / {color}");
		block.SetColor(color);
		return block;
	}
	public void DeSpawnBlock(Block block)
	{
		ObjectPoolManager.Instance.Despawn(block.GetComponent<PoolObject>());
	}
}
