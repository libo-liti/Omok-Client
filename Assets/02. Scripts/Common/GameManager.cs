using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    // Main Scene에서 선택한 게임 타입
    public Constants.GameType _gameType = Constants.GameType.DualPlay;

    /*[SerializeField] private GameObject optionPanel; //설정 메뉴 패널*/
    [SerializeField] private GameObject confirmPanel; //안내 패널
    [SerializeField] private GameObject loginPanel; //로그인 화면
    [SerializeField] private GameObject askPanel; // 여부 묻기 패널
    [SerializeField] private GameObject gameSelectPanel; //게임 선택화면
    
    public string guestName = null;
    public bool IsGuestLoggedIn => !string.IsNullOrEmpty(guestName);


    // Panel을 띄우기 위한 Canvas 정보
    private Canvas _canvas;

    // Game Logic
    private GameLogic _gameLogic;

    // Main Scene 구현 전이므로 OnSceneLoad 함수가 실행되지 않음
    // 임시로 Start 함수에서 게임 초기화
    private void Start()
    {
        /*
         _canvas = FindFirstObjectByType<Canvas>();
        // MainSceneTest 임시 설정 -> MainSceneTest 일때는 Start 건너뜁니다
        if (_gameType == Constants.GameType.MainSceneTest) return;

        PointController pointController = FindFirstObjectByType<PointController>();
        if (pointController != null)
        {
            Debug.Log(pointController);
            pointController.InitPoints();
            _gameLogic = new GameLogic(pointController, _gameType);
        }*/
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
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.name == "Main")
            {
                Debug.Log("1");
                if (IsGuestLoggedIn)
                {
                    Debug.Log("2");
                    if (loginPanel != null) loginPanel.SetActive(false);
                    if (gameSelectPanel != null) gameSelectPanel.SetActive(true);
                }
                else
                {
                    if (gameSelectPanel != null) gameSelectPanel.SetActive(false);
                    if (loginPanel != null) loginPanel.SetActive(true);
                }
            }
        };
    }


    protected override void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        _canvas = FindFirstObjectByType<Canvas>();

        if (scene.name == "Game")
        {
            PointController pointController = FindFirstObjectByType<PointController>();
            Timer timer = FindFirstObjectByType<Timer>();
            EmojiController emojiController = FindFirstObjectByType<EmojiController>();
            if (pointController != null && emojiController != null)
            {
                pointController.InitPoints();
                emojiController.Init();
                _gameLogic = new GameLogic(pointController, emojiController ,timer, _gameType);
                
                Debug.Log(_gameType);
            }
        }
    }

    public void OpenPanel(GameObject panelPrefab)
    {
        if (_canvas != null && panelPrefab != null)
        {
            Instantiate(panelPrefab, _canvas.transform);
        }
    }

    public void OpenConfirmPanel(string message, System.Action onConfirm)
    {
        if (_canvas != null)
        {
            var confirmPanelObject = Instantiate(confirmPanel, _canvas.transform);
            confirmPanelObject.GetComponent<ConfirmPanelController>()
                .Show(message, onConfirm);
        }
    }
    
    public void OpenAskPanel(string message, System.Action yes, System.Action no)
    {
        if (_canvas != null && askPanel != null)
        {
            var askPanelObject = Instantiate(askPanel, _canvas.transform);
            askPanelObject.GetComponent<AskPanelController>()
                .Show(message, yes, no);
        }
    }


    public void GuestLogin()
    {
        //필요없어 보여서 임시 주석
        /*if (loginPanel != null) loginPanel.SetActive(false);
        if (gameSelectPanel != null) gameSelectPanel.SetActive(true);*/
        
        //로그인 여부 판단을 위해서 임시 생성 언제활용할지 모르니 일단 랜던아이디만 부여
        // 활용한다면 나중에 중복 확인도 필요할듯?
        if (string.IsNullOrEmpty(guestName))
        {
            guestName = "비회원_" + UnityEngine.Random.Range(1000, 9999);
        }
        Debug.Log(guestName);
    }

    public void Logout()
    {
        guestName = null;
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료");

        // 에디터일때는 PlayMode 중지
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    // 빌드 후엔 게임 종료
    Application.Quit();
#endif
    }
    
    private void OnApplicationQuit()
    {
        _gameLogic?.Dispose();
        _gameLogic = null;
    }
}