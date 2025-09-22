using System.Collections;
using TMPro;
using UnityEngine;

public class MainSceneUIManager : MonoBehaviour
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject gameSelectPanel;
    [SerializeField] private TMP_Text userNameText;
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private GameObject loginGroup;
    
    private bool isFading = false;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        RefreshUI();
    }

    public void OnClickGuestLogin()
    {
        emailField.text = "";
        passwordInputField.text = "";
        GameManager.Instance.GuestLogin();
        RefreshUI();
    }

    public void OnClickLogout()
    {
        emailField.text = "";
        passwordInputField.text = "";
        GameManager.Instance.Logout();
        RefreshUI();
    }

    public void OnClickQuitGame()
    {
        GameManager.Instance.QuitGame();
        RefreshUI();
    }
    
    public void OnClickOpenPanel(GameObject panelPrefab)
    {
        
        // 현재 Canvas 안에 같은 이름의 패널이 있는지 찾음
        Transform existingPanel = GameManager.Instance.GetCanvas().transform.Find(panelPrefab.name + "(Clone)");
        
        Debug.Log($"12345 {panelPrefab.name}");

        if (existingPanel != null)
        {
            Destroy(existingPanel.gameObject);
        }
        else
        {
            GameManager.Instance.OpenPanel(panelPrefab);
            
            if (panelPrefab.name == "JoinGroup" )
            {
                SetLoginPanelVisible(false);
            }
        }
        

        RefreshUI();
    }


    public void RefreshUI()
    {
        bool isGuest = GameManager.Instance.IsGuestLoggedIn;

        loginPanel.SetActive(!isGuest);
        gameSelectPanel.SetActive(isGuest);

        if (userNameText != null)
            userNameText.text = isGuest ? GameManager.Instance.GetGuestName() : "";
    }
    public void SetLoginPanelVisible(bool visible)
    {
        if (loginGroup != null)
        {
            CanvasGroup canvasGroup = loginGroup.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = loginGroup.AddComponent<CanvasGroup>();

            if (visible)
            {
                loginGroup.SetActive(true);

                if (!isFading) // 이미 페이드 중이면 무시
                    fadeCoroutine = StartCoroutine(FadeIn(canvasGroup, 0.5f));
            }
            else
            {
                if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
                isFading = false;
                loginGroup.SetActive(false);
            }
        }
    }
    private IEnumerator FadeIn(CanvasGroup canvasGroup, float duration)
    {
        isFading = true;
        float time = 0f;
        canvasGroup.alpha = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, time / duration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        isFading = false;
    }
}

