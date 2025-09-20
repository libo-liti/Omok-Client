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

    /*[SerializeField] private Button exitButton;*/
    [SerializeField] private Button surrenderButton;
    [SerializeField] private TMP_Text userNameText;
    public TMP_Text opponentNameText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        /*// 시작 시에는 비활성화
        if (resultText != null)
            resultText.gameObject.SetActive(false);
        if (exitButton != null)
            exitButton.gameObject.SetActive(false);
        if (surrenderButton != null)
            surrenderButton.gameObject.SetActive(true);*/
    }

    private void Start()
    {
        if (userNameText != null && GameManager.Instance.IsGuestLoggedIn)
            userNameText.text = GameManager.Instance.GetGuestName();
        
        /*exitButton.onClick.AddListener(() =>
        {
            Debug.Log("누름");
            ExitRoomAction?.Invoke();
            if(GameManager.Instance._gameType != Constants.GameType.MultiPlay)
                GameManager.Instance.ChangeToMainScene();
        });*/
        
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

    /*public void ShowButton(GameLogic.GameResult gameResult)
    {
        if (gameResult != GameLogic.GameResult.None)
        {
            exitButton.gameObject.SetActive(true);
            surrenderButton.interactable = false;
        }
    }*/
    public void OnClickOpenPanel(GameObject panelPrefab)
    {
        // 현재 Canvas 안에 같은 이름의 패널이 있는지 찾음
        Transform existingPanel = GameManager.Instance.GetCanvas().transform.Find(panelPrefab.name + "(Clone)");

        if (existingPanel != null)
        {
            Destroy(existingPanel.gameObject);
        }
        else
        {
            GameManager.Instance.OpenPanel(panelPrefab);
        }
    }

    public void RematchCheck()
    {
        GameManager.Instance.OpenAskPanel("재경기 하시겠습니까?", () =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        },() =>
        {
            
        });

        /*Destroy(gameObject);*/
    }
}