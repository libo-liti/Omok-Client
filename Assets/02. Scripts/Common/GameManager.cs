using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    // Main Scene에서 선택한 게임 타입
	[SerializeField]
    private Constants.GameType _gameType = Constants.GameType.DualPlay;
    
    // Panel을 띄우기 위한 Canvas 정보
    private Canvas _canvas;
    
    // Game Logic
    private GameLogic _gameLogic;

    // Main Scene 구현 전이므로 OnSceneLoad 함수가 실행되지 않음
    // 임시로 Start 함수에서 게임 초기화
    private void Start()
    {
        PointController pointController = FindFirstObjectByType<PointController>();
        pointController.InitPoints();
        _gameLogic = new GameLogic(pointController, _gameType);
    }


    /// <summary>
    /// Main에서 Game Scene으로 전환시 호출될 메서드
    /// </summary>
    public void ChangeToGameScene(Constants.GameType gameType)
    {
        _gameType = gameType;
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// Game에서 Main Scene으로 전환시 호출될 메서드
    /// </summary>
    public void ChangeToMainScene()
    {
        SceneManager.LoadScene("Main");
    }
    

    protected override void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        // PointController pointController = FindFirstObjectByType<PointController>();
        // pointController.InitPoints();
        //
        // _canvas = FindFirstObjectByType<Canvas>();
        //
        // if (scene.name == "Game")
        // {
        //     // GameLogic 생성
        //     _gameLogic = new GameLogic(pointController, _gameType);
        // }
    }
}
