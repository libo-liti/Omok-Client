using System;
using UnityEngine;

namespace jong
{
    public class GameManager : Singleton<GameManager>
    {
        public enum Turn { PlayerA = 1, PlayerB }
        public Turn currentTurn = Turn.PlayerA;
        
        public enum GameResult { Win, Lose, Draw, None }
    
        public Board board;
        private AI _ai;
        
                                   // 우하, 우, 우상, 상
        private int[] _dx = new int[4] { 1, 1, 1, 0 };
        private int[] _dy = new int[4] { -1, 0, 1, 1 };
    
        private void Start()
        {
            _ai = new AI();
        }
    
        // public void AITurn()
        // {
        //     ChangeTurn();
        //     var pos = _ai.GetBestMove(board.board);
        //     Debug.Log($"y : {pos.y} x : {pos.x}");
        //     Debug.Log($"y : {pos.y} x : {pos.x}");
        // }
    
        public void ChangeTurn()
        {
            if (currentTurn == Turn.PlayerA)
                currentTurn = Turn.PlayerB;
            else
                currentTurn = Turn.PlayerA;
        }
    
        public GameResult CheckWin(int y, int x, int player)
        {
            for (int i = 0; i < 4; i++)
            {
                int count = 1;
                // 순방향
                for (int j = 1; j <= 5; j++)
                {
                    int nx = x + _dx[i] * j;
                    int ny = y + _dy[i] * j;
                    
                    if (nx < 0 || board.boardSize < nx || ny < 0 || board.boardSize < ny)
                        break;
                    if (board.board[ny, nx] != player)
                        break;
                    count++;
                }
                // 역방향
                for (int j = 1; j <= 5; j++)
                {
                    int nx = x - _dx[i] * j;
                    int ny = y - _dy[i] * j;
                    
                    if (nx < 0 || board.boardSize < nx || ny < 0 || board.boardSize < ny)
                        break;
                    if (board.board[ny, nx] != player)
                        break;
                    count++;
                }
    
                if (count == 5)
                    return GameResult.Win;
            }
            
            // 모든 판을 채웠는지
            for (int i = 0; i < board.boardSize; i++)
                for(int j = 0; j < board.boardSize; j++)
                    if(board.board[i, j] == 1 || board.board[i, j] == 2)
                        return GameResult.None;
    
            return GameResult.Draw;
        }
    }
}
