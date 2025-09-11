using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MultiplayerState : BasePlayerState
{
    private Constants.PlayerType _playerType;
    private bool _isFirstPlayer;
    private NetworkManager _networkManager;

    public MultiplayerState(bool isFirstPlayer, NetworkManager networkManager)
    {
        Debug.Log("MultiPlayer");
        _isFirstPlayer = isFirstPlayer;
        _networkManager = networkManager;
        _playerType = _isFirstPlayer ? Constants.PlayerType.PlayerA : Constants.PlayerType.PlayerB;
    }
    
    public override void OnEnter(GameLogic gameLogic)
    {
        _networkManager.onBlockDataChanged = blockIndex =>
        {
            var row = blockIndex / Constants.BoardSize;
            var col = blockIndex % Constants.BoardSize;
            UnityThread.executeInUpdate(() =>
            {
                HandleMove(gameLogic, row, col);
            });
        };
    }

    public override void OnExit(GameLogic gameLogic)
    {
        _networkManager.onBlockDataChanged = null;
    }

    public override void HandleMove(GameLogic gameLogic, int row, int col)
    {
        ProcessMove(gameLogic, _playerType, row, col);
    }

    protected override void HandleNextTurn(GameLogic gameLogic)
    {
        if (_isFirstPlayer)
            gameLogic.SetState(gameLogic.secondPlayerState);
        else
            gameLogic.SetState(gameLogic.firstPlayerState);
    }
}
