using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class AIState : BasePlayerState
{
    private bool _isFirstPlayer;
    private Constants.PlayerType _playerType;
    public AIState(bool isFirstPlayer)
    {
        _isFirstPlayer = isFirstPlayer;
        _playerType = _isFirstPlayer ? Constants.PlayerType.PlayerA : Constants.PlayerType.PlayerB;
    }

    public override void OnEnter(GameLogic gameLogic)
    {
        gameLogic.timer.StartTimer(_isFirstPlayer, () =>
        {
            gameLogic.EndGame(_isFirstPlayer ? GameLogic.GameResult.Lose : GameLogic.GameResult.Win);
        });

        
        var board = gameLogic.GetBoard();
        var result = OmokAI.GetBestMove(board, out int bestScore);
        if (result.HasValue)
        {
            // AI가 상황에 따라 확률적으로 이모티콘을 사용
            if (bestScore > 1000000)
                WaitAndShowEmoji(gameLogic._emojiController, true);
            else if (bestScore < -1000000)
                WaitAndShowEmoji(gameLogic._emojiController, false);
                
            WaitAndProceed(gameLogic, result.Value.row, result.Value.col);
        }
        else
        {
            gameLogic.EndGame(GameLogic.GameResult.Draw);
        }
    }

    public override void OnExit(GameLogic gameLogic)
    {
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

    // AI가 자연스럽게 잠깐 대기 후 착수
    private async void WaitAndProceed(GameLogic gameLogic, int row, int col)
    {
        // 1 ~ 2초 대기 후 착수
        int randomMilliseconds = Random.Range(1000, 2000);
        await Task.Delay(randomMilliseconds);
        
        HandleMove(gameLogic, row, col);
    }

    // AI가 잠깐 대기 후 상황에 맞는 이모티콘을 사용함
    private async void WaitAndShowEmoji(EmojiController emojiController, bool isWin)
    {
        // 75% 확률로 이모티콘 미사용
        int random = Random.Range(0, 100);
        if (random < 75)
            return;
        
        // AI 턴이 된 후 0.5초 대기
        await Task.Delay(500);
        
        int[] winEmoji = { 0, 3, 6, 7, 9 }; // 이기고 있을 때 사용하는 이모티콘 인덱스 목록
        int[] loseEmoji = { 1, 4, 8, 14, 15 }; // 지고 있을 때 사용하는 이모티콘 인덱스 목록
        
        int randomIndex = Random.Range(0, 5);
        int emojiIndex = isWin ? winEmoji[randomIndex] : loseEmoji[randomIndex];
        emojiController.SetEmoji(emojiIndex, false);
    }
}
