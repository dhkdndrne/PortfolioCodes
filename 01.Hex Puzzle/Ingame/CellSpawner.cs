using System.Collections;
using System.Collections.Generic;
using Bam.Singleton;
using UnityEngine;

public class CellSpawner : Singleton<CellSpawner>
{
    private readonly string CELL_POOLING_KEY = "Cell";

    // public Block SpawnCell(BlockData data)
    // {
    //     var blockObject = ObjectPoolManager.Instance.Spawn(CELL_POOLING_KEY);
		  //
    //     var block = blockObject.GetComponent<Block>();
    //     block.SetData(data);
		  //
    //     return block;
    // }
}
