using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace jong
{
    public class Player : MonoBehaviour
    {
        public static GameObject currentStone;

        public Board board;
        private Camera _camera;
        private Vector2Int _pos;
        private bool _isPlay;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !_isPlay)    // 마우스 눌렸을때 투명한 바둑 생성
            {
                // 바둑판 범위 안
                var mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
                if ((mousePos.x < 0 || board.boardSize < mousePos.x) || (mousePos.y < 0 || board.boardSize < mousePos.y))
                    return;
            
                _pos = GetStonePosition();
                if (board.IsVisited(_pos))
                    return;
            
                board.Preview(_pos);
                _isPlay = true;
            }
            else if (Input.GetMouseButton(0) && _isPlay)    // 마우스 드래그 중에 돌 움직임
            {
                _pos = GetStonePosition();
                if (board.IsVisited(_pos))
                    return;
            
                currentStone.transform.position = (Vector3Int)_pos;
            }
            else if (Input.GetMouseButtonUp(0) && _isPlay)  // 마우스 땠을때 바둑판 위에 돌 위치 시킴
            {
                board.SetStone(_pos);
                _isPlay = false;
            }
        }

        /// <summary>
        /// 마우스 위치를 바둑알이 놓일 위치로 변환
        /// </summary>
        /// <returns></returns>
        private Vector2Int GetStonePosition()
        {
            // 마우스 위치 -> 2차원 좌표
            Vector2 pos = _camera.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int newPos = Vector2Int.zero;
        
            // 바둑판 모서리로 이동
            newPos.x = (int)Mathf.Round(pos.x);
            newPos.y = (int)Mathf.Round(pos.y);

            // 바둑판 범위체크
            newPos.x = Mathf.Clamp(newPos.x, 0, board.boardSize);
            newPos.y = Mathf.Clamp(newPos.y, 0, board.boardSize);
        
            return newPos;
        }
    }
}
