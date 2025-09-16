using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneUIManager : MonoBehaviour
{
    public static GameSceneUIManager Instance; 

    [SerializeField] 
    private TMP_Text resultText;

    [SerializeField] 
    private Button exitButton;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 시작 시에는 비활성화
        if (resultText != null)
            resultText.gameObject.SetActive(false);
        if (exitButton != null)
            exitButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        exitButton.onClick.AddListener(() =>
        {
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
        }
    }
}