using System;
using System.Collections.Generic;
using UnityEngine;

namespace jong
{
    public class Board : MonoBehaviour
    {
        public List<Vector2> record = new List<Vector2>();
        public int[,] board;
        public int boardSize = 15;
        
        private SpriteRenderer stoneSprite;
        
        public GameObject whiteStone;
        public GameObject blackStone;
    
        private void Start()
        {
            board = new int[boardSize + 1, boardSize + 1];
        }
    
        // 바둑돌 놓을 위치 미리보기
        public void Preview(Vector2 pos)
        {
            // 바둑돌 생성
            if (GameManager.Instance.currentTurn == GameManager.Turn.PlayerA)
                Player.currentStone = Instantiate(blackStone, pos, Quaternion.identity);
            else
                Player.currentStone = Instantiate(whiteStone, pos, Quaternion.identity);
            
            // 바둑돌 투명하게
            stoneSprite = Player.currentStone.GetComponent<SpriteRenderer>();
            stoneSprite.color = ChangeAlpha(stoneSprite, 0.6f);
        }
        
        // 바둑돌 놓기
        public void SetStone(Vector2Int pos)
        {
            // 바둑돌이 놓인 위치를 저장
            board[pos.y, pos.x] = (int)GameManager.Instance.currentTurn;
            record.Add(pos);
            
            // 바둑돌을 board 자식으로 두기
            Player.currentStone.transform.SetParent(transform);
            
            // 바둑돌의 투명도를 원래대로
            stoneSprite = Player.currentStone.GetComponent<SpriteRenderer>();
            stoneSprite.color = ChangeAlpha(stoneSprite, 1f);
    
            switch (GameManager.Instance.CheckWin(pos.y, pos.x, (int)GameManager.Instance.currentTurn))
            {
                case GameManager.GameResult.Win:
                    Debug.Log(GameManager.Instance.currentTurn + " Win");
                    break;
                case GameManager.GameResult.Draw:
                    Debug.Log("Draw");
                    break;
            }
            GameManager.Instance.ChangeTurn();
            // GameManager.Instance.AITurn();
        }
        
        // 투명도 변화
        private Color ChangeAlpha(SpriteRenderer sprite, float a)
        {
            var color = sprite.color;
            color.a = a;
            sprite.color = color;
            return color;
        }
        
        // 바둑돌이 놓여 있는지 체크
        public bool IsVisited(Vector2 pos)
        {
            var result = board[(int)pos.y, (int)pos.x] != 0;
            return result;
        }
    }
}
