using UnityEngine;

public class PlayerState : BasePlayerState
{
    private bool _isFirstPlayer;
    private Constants.PlayerType _playerType;
    private MultiplayController _multiplayController;
    private EmojiController _emojiController;

    private bool _isMultiplay;
    private string _roomId;
    private bool[,] forbiddenPoints;
    
    public PlayerState(bool isFirstPlayer)
    {
        _isFirstPlayer = isFirstPlayer;
        _playerType = _isFirstPlayer ? 
            Constants.PlayerType.PlayerA :  Constants.PlayerType.PlayerB;
    }

    public PlayerState(bool isFirstPlayer, MultiplayController multiplayController, EmojiController emojiController,
        string roomId) : this(isFirstPlayer)
    {
        _multiplayController = multiplayController;
        _emojiController = emojiController;
        _roomId = roomId;
        _isMultiplay = true;
        _emojiController.EmitEmoji = (n) =>
        {
            _multiplayController.PlayerEmoji(_roomId, n);
        };
        _multiplayController.setEmoji = (n) =>
        {
            _emojiController.SetEmoji(n, false);
        };
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
        gameLogic.pointController.OnPointEnterDelegate = (row, col) =>
        {
            gameLogic.Preview(_playerType, row, col, true);
        };
        gameLogic.pointController.OnPointExitDelegate = (row, col) =>
        {
            gameLogic.Preview(_playerType, row, col, false);
        };
        
        // 3. Timer 시작 및 이벤트 전달
        if (_isMultiplay)
        {
            UnityThread.executeInUpdate(() =>
            {
                gameLogic.timer.StartTimer(_isFirstPlayer, () =>
                {
                    gameLogic.EndGame(_isFirstPlayer ? GameLogic.GameResult.Lose : GameLogic.GameResult.Win);
                });
            });
        }
        else
        {
            gameLogic.timer.StartTimer(_isFirstPlayer, () =>
            {
                gameLogic.EndGame(_isFirstPlayer ? GameLogic.GameResult.Lose : GameLogic.GameResult.Win);
            });
        }
        
        // 4. 흑돌이라면 렌주룰에 의한 금수 위치를 표시
        if (_isFirstPlayer)
        {
            forbiddenPoints = OmokAI.IsForbidden(gameLogic.GetBoard());
            gameLogic.SetForbiddenPoint(forbiddenPoints);
        }
    }

    public override void OnExit(GameLogic gameLogic)
    {
        gameLogic.pointController.OnPointClickedDelegate = null;
        gameLogic.pointController.OnPointEnterDelegate = null;
        gameLogic.pointController.OnPointExitDelegate = null;
        gameLogic.timer.StopTimer(); // 타이머 비활성화
        if (_isFirstPlayer) gameLogic.RemoveAllForbiddenPoint(); // 흑돌 턴 종료시 모든 금수 표시 비활성화
    }

    public override void HandleMove(GameLogic gameLogic, int row, int col)
    {
        // 흑돌은 금수 위치에는 착수 불가
        if (_isFirstPlayer && forbiddenPoints[row, col])
            return;
        
        ProcessMove(gameLogic, _playerType, row, col);
        
        if (_isMultiplay)
        {
            _multiplayController.DoPlayer(_roomId, col, row);
        }
    }

    protected override void HandleNextTurn(GameLogic gameLogic)
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
