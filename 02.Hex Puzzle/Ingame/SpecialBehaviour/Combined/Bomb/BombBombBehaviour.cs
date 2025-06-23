using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BombBombBehaviour : MonoBehaviour,ISpecialBlockBehaviour
{
    private Queue<Block> q = new();
    private HashSet<Block> hashSet;
    public void Execute(Hex hex,SpecialBlockType blockType, HashSet<Block> withinRangeList)
    {
        var board = GameManager.Instance.Board;
        q.Clear();
        q.Enqueue(board.GetBlock(hex));

        int cnt = 1;
        int cycle = 2;
		
        while (cycle > 0)
        {
            while (cnt > 0)
            {
                var b = q.Dequeue();
                cnt--;
                for (int i = 0; i < (int)HexWay.Length; i++)
                {
                    var nHex = Hex.GetHexByWay(b.Hex, (HexWay)i);
                    if (board.IsIndexOutOfRange(nHex)) continue;
                    if (!board.IsValidIndex(nHex)) continue;

                    var nb = board.GetBlock(nHex);
                    if (nb == null) continue;
                    
                    withinRangeList.Add(nb);
                    q.Enqueue(nb);
                }
            }
            cnt = q.Count;
            cycle--;
        }

        hashSet = withinRangeList;
    }

    public async UniTask Anim(Block block)
    {
        await GameManager.Instance.Board.ShowTargetHighlightAll(hashSet);
    }
}
