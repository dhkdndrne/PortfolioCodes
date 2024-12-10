using System.Collections;
using System.Collections.Generic;
using Bam.Singleton;
using UnityEngine;

public class GameManager : Singleton<GameManager>,IManger
{
    [SerializeField] private CameraController cc;
    [SerializeField] private StageData temp;
   
    private StateMachine stateMachine;
    private Board board;
    
    public Board Board => board;
    public bool IsLoaded { get; private set; }
    
    public void InitManager()
    {
        Application.targetFrameRate = 60;

        if (board == null)
            board = FindFirstObjectByType<Board>();
        
        if (stateMachine == null)
            stateMachine = FindFirstObjectByType<StateMachine>();
        
        Stage.Instance.LoadStage(temp);
        board.InitBoard(temp);
        cc.FitCamera(board.Row,board.RowGap);

        IsLoaded = true;
        stateMachine.StartFsm();
    }
}
