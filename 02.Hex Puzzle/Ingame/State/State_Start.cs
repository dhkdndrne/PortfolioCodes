using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using DG.Tweening;

public class State_Start : State
{
    private void Awake()
    {
       
        OnBeginStream.Subscribe(async _=>
        {
            
            
            //var board = Stage.Instance.Board;
            // List<UniTask> tasks = new List<UniTask>();
            //
            // foreach (var block in board.GetBlockEnumerable())
            // {
            //     Tween tween = block.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.Linear).From(Vector3.zero);
            //     tasks.Add(tween.ToUniTask());
            // }
            //
            // foreach (var src in board.GetCellEnumerable())
            // {
            //     Tween tween = src.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.Linear).From(Vector3.zero);
            //     tasks.Add(tween.ToUniTask());
            // }
            //
            // await UniTask.WhenAll(tasks);
            ChangeState<State_Input>();

        }).AddTo(this);
        
    }
}
