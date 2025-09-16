using UnityEngine;

using Unity.VisualScripting;
using UnityEngine;

public class PlayerState : BasePlayerState
{
    private bool _isFirstPlayer;
    private Constants.PlayerType _playerType;
    private MultiplayController _multiplayController;

    private bool _isMultiplay;
    private string _roomId;
    
    public PlayerState(bool isFirstPlayer)
    {
        _isFirstPlayer = isFirstPlayer;
        _playerType = _isFirstPlayer ? 
            Constants.PlayerType.PlayerA :  Constants.PlayerType.PlayerB;
    }

    public PlayerState(bool isFirstPlayer, MultiplayController multiplayController, string roomId) : this(isFirstPlayer)
    {
        _multiplayController = multiplayController;
        _roomId = roomId;
        _isMultiplay = true;
    }
    
    public override void OnEnter(GameLogic gameLogic)
    {
        // 1. First Player인지 확인해서 게임 UI에 현재 턴 표시
        Debug.Log(_playerType + "'s turn");

        // 2. Point Controller에게 해야 할 일을 전달
        gameLogic.pointController.OnPointClickedDelegate = (row, col) =>
        {
            // Point가 터치 될 때까지 기다렸다가 터치 되면 처리할 일
            HandleMove(gameLogic, row, col);
        };
    }

    public override void OnExit(GameLogic gameLogic)
    {
        gameLogic.pointController.OnPointClickedDelegate = null;
    }

    public override void HandleMove(GameLogic gameLogic, int row, int col)
    {
        ProcessMove(gameLogic, _playerType, row, col);
        if (_isMultiplay)
        {
            _multiplayController.DoPlayer(_roomId, col, row);
        }
    }

    public override void HandleNextTurn(GameLogic gameLogic)
    {
        if (_isFirstPlayer)
        {
            gameLogic.SetState(gameLogic.secondPlayerState);
        }
        else
        {
            gameLogic.SetState(gameLogic.firstPlayerState);
        }
    }
}
