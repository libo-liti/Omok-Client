using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneUIManager : MonoBehaviour
{
    public static GameSceneUIManager Instance;

    public Action SurrenderAction;
    public Action ExitRoomAction;
    
    [SerializeField] private TMP_Text resultText;

    [SerializeField] private Button exitButton;
    [SerializeField] private Button surrenderButton;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 시작 시에는 비활성화
        if (resultText != null)
            resultText.gameObject.SetActive(false);
        if (exitButton != null)
            exitButton.gameObject.SetActive(false);
        if (surrenderButton != null)
            surrenderButton.gameObject.SetActive(true);
    }

    private void Start()
    {
        exitButton.onClick.AddListener(() =>
        {
            ExitRoomAction?.Invoke();
            if(GameManager.Instance._gameType != Constants.GameType.MultiPlay)
                GameManager.Instance.ChangeToMainScene();
        });
        
        surrenderButton.onClick.AddListener(() =>
        {
            SurrenderAction?.Invoke();
            if(GameManager.Instance._gameType != Constants.GameType.MultiPlay)
                GameManager.Instance.ChangeToMainScene();
        });
    }

    public void ShowResult(string message, Color fontColor)
    {
        if (resultText != null)
        {
            resultText.text = message;
            resultText.color = fontColor;
            resultText.gameObject.SetActive(true);
        }
    }

    public void ShowButton(GameLogic.GameResult gameResult)
    {
        if (gameResult != GameLogic.GameResult.None)
        {
            exitButton.gameObject.SetActive(true);
            surrenderButton.interactable = false;
        }
    }
    public void OnClickOpenPanel(GameObject panelPrefab)
    {
        GameManager.Instance.OpenPanel(panelPrefab);
    }

    public void RematchCheck()
    {
        GameManager.Instance.OpenAskPanel("재경기 하시겠습니까?", () =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        },() =>
        {
            
        });

        Destroy(gameObject);
    }
}