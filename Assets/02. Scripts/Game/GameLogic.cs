
using System;
using TMPro;
using UnityEngine;

public class GameLogic
{
    public PointController pointController;         // Point를 처리할 객체

    private Constants.PlayerType[,] _board;         // 보드의 상태 정보
    
    public BasePlayerState firstPlayerState;        // Player A
    public BasePlayerState secondPlayerState;       // Player B

    public enum GameResult { None, Win, Lose, Draw }
    
    private BasePlayerState _currentPlayerState;    // 현재 턴의 Player
    private TMP_Text _resultText;
    Color fontColor = Color.white;
    private MultiplayController _multiplayController;
    private string _roomId;

    public GameLogic(PointController pointController, Constants.GameType gameType)
    {
        this.pointController = pointController;
        
        // 보드의 상태 정보 초기화
        _board = 
            new Constants.PlayerType[Constants.BoardSize, Constants.BoardSize];
        
        // Game Type 초기화
        switch (gameType)
        {
            case Constants.GameType.SinglePlay:
                firstPlayerState = new PlayerState(true);
                secondPlayerState = new AIState();
                // 게임 시작
                SetState(firstPlayerState);
                break;
            case Constants.GameType.DualPlay:
                firstPlayerState = new PlayerState(true);
                secondPlayerState = new PlayerState(false);
                // 게임 시작
                SetState(firstPlayerState);
                break;
            case Constants.GameType.MultiPlay:
                _multiplayController = new MultiplayController((state, roomId) =>
                {
                    _roomId = roomId;
                    switch (state)
                    {
                        case Constants.MultiplayControllerState.CreateRoom:
                            Debug.Log("## CreateRoom ##");
                            firstPlayerState = new PlayerState(true, _multiplayController, _roomId);
                            secondPlayerState = new MultiplayerState(false, _multiplayController);
                            SetState(firstPlayerState);
                            break;
                        case Constants.MultiplayControllerState.JoinRoom:
                            Debug.Log("## JoinRoom ##");
                            firstPlayerState = new MultiplayerState(true, _multiplayController);
                            secondPlayerState = new PlayerState(false, _multiplayController, _roomId);
                            SetState(firstPlayerState);
                            break;
                        case Constants.MultiplayControllerState.GameStart:
                            Debug.Log("## GameStart ##");
                            break;
                        case Constants.MultiplayControllerState.EndGame:
                            break;
                    }
                });
                break;
        }
    }

    public Constants.PlayerType[,] GetBoard()
    {
        return _board;
    }

    // 턴이 바뀔 때, 기존 진행하던 상태를 Exit 하고
    // 이번 턴의 상태를 _currentPlayerState에 할당하고
    // 이번 턴의 상탱에 Enter 호출
    public void SetState(BasePlayerState state)
    {
        _currentPlayerState?.OnExit(this);
        _currentPlayerState = state;
        _currentPlayerState?.OnEnter(this);
    }
    
    // _board 배열에 새로운 Marker 값을 할당
    public bool SetNewBoardValue(Constants.PlayerType playerType,
        int row, int col)
    {
        if (_board[row, col] != Constants.PlayerType.None) return false;
        
        if (playerType == Constants.PlayerType.PlayerA)
        {
            _board[row, col] = playerType;
            pointController.PlaceMaker(Point.MarkerType.Black, row, col);
            return true;
        }
        else if (playerType == Constants.PlayerType.PlayerB)
        {
            _board[row, col] = playerType;
            pointController.PlaceMaker(Point.MarkerType.White, row, col);
            return true;
        }        
        return false;
    }
    
    // Game Over 처리
    public void EndGame(GameResult gameResult)
    {
        SetState(null);
        firstPlayerState = null;
        secondPlayerState = null;
        
        string message = "";
        switch (gameResult)
        {
            case GameResult.Win:
                message = "흑돌 승";
                fontColor = Color.black;
                break;
            case GameResult.Lose:
                message = "백돌 승";
                fontColor = Color.white;
                break;
        }

        GameSceneUIManager.Instance.ShowResult(message, fontColor);
        GameSceneUIManager.Instance.ShowButton(gameResult);

        // 유저에게 Game Over 표시
        Debug.Log("게임 결과 : " + gameResult);

    }
    
    // 게임의 결과 확인
    public GameResult CheckGameResult(int y, int x)
    {
        if (OmokAI.CheckWin(_board, Constants.PlayerType.PlayerA, y, x)) { return GameResult.Win; }
        if (OmokAI.CheckWin(_board, Constants.PlayerType.PlayerB, y, x)) { return GameResult.Lose; }
        if (OmokAI.IsBoardFull(_board)) { return GameResult.Draw; }
        return GameResult.None;
    }

    public void Dispose()
    {
        _multiplayController.Dispose();
    }
}
