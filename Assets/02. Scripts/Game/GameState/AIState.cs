using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class AIState : BasePlayerState
{
    public override void OnEnter(GameLogic gameLogic)
    {
        gameLogic.timer.StartTimer(false, () =>
        {
            gameLogic.EndGame(GameLogic.GameResult.Win);
        });
        
        var board = gameLogic.GetBoard();
        var result = OmokAI.GetBestMove(board);
        if (result.HasValue)
        {
            // HandleMove(gameLogic, result.Value.row, result.Value.col);
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
        ProcessMove(gameLogic, Constants.PlayerType.PlayerB, row, col);
    }

    protected override void HandleNextTurn(GameLogic gameLogic)
    {
        gameLogic.SetState(gameLogic.firstPlayerState);
    }

    // private IEnumerator WaitAndProceed(GameLogic gameLogic, int row, int col)
    // {
    //     float randomTime = Random.Range(1, 3);
    //     yield return new WaitForSeconds(randomTime);
    //     
    //     HandleMove(gameLogic, row, col);
    // }

    private async void WaitAndProceed(GameLogic gameLogic, int row, int col)
    {
        // 1 ~ 2초 대기 후 착수
        int randomMilliseconds = Random.Range(1000, 2000);
        await Task.Delay(randomMilliseconds);
        
        HandleMove(gameLogic, row, col);
    }
}
