using UnityEngine;

public class MainSceneUIManager : MonoBehaviour
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject gameSelectPanel;

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
        if (GameManager.Instance.IsGuestLoggedIn)
        {
            loginPanel.SetActive(false);
            gameSelectPanel.SetActive(true);
        }
        else
        {
            loginPanel.SetActive(true);
            gameSelectPanel.SetActive(false);
        }
    }
}

