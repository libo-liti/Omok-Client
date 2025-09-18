using TMPro;
using UnityEngine;

public class MainSceneUIManager : MonoBehaviour
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject gameSelectPanel;
    [SerializeField] private TMP_Text userNameText;

    private void Start()
    {
        RefreshUI();
    }

    public void OnClickGuestLogin()
    {
        GameManager.Instance.GuestLogin();
        RefreshUI();
    }

    public void OnClickLogout()
    {
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
        GameManager.Instance.OpenPanel(panelPrefab);
        RefreshUI();
    }

    private void RefreshUI()
    {
        bool isGuest = GameManager.Instance.IsGuestLoggedIn;

        loginPanel.SetActive(!isGuest);
        gameSelectPanel.SetActive(isGuest);

        if (userNameText != null)
            userNameText.text = isGuest ? GameManager.Instance.GetGuestName() : "";
    }
}

