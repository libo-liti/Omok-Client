using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MultiplayerState : BasePlayerState
{
    private Constants.PlayerType _playerType;
    private bool _isFirstPlayer;
    private MultiplayController _multiplayController;

    public MultiplayerState(bool isFirstPlayer, MultiplayController multiplayController)
    {
        _isFirstPlayer = isFirstPlayer;
        _multiplayController = multiplayController;
        _playerType = _isFirstPlayer ? Constants.PlayerType.PlayerA : Constants.PlayerType.PlayerB;
    }
    
    public override void OnEnter(GameLogic gameLogic)
    {
        _multiplayController.onBlockDataChanged = blockIndex =>
        {
            var row = blockIndex / Constants.BoardSize;
            var col = blockIndex % Constants.BoardSize;
            UnityThread.executeInUpdate(() =>
            {
                HandleMove(gameLogic, row, col);
            });
        };
        
        gameLogic.timer.StartTimer(_isFirstPlayer, () =>
        {
            gameLogic.EndGame(_isFirstPlayer ? GameLogic.GameResult.Lose : GameLogic.GameResult.Win);
        });
    }

    public override void OnExit(GameLogic gameLogic)
    {
        _multiplayController.onBlockDataChanged = null;
        gameLogic.timer.StopTimer();
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
