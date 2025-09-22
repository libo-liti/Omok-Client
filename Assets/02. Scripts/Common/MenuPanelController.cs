using UnityEngine;
using UnityEngine.UI;

public class MenuPanelController : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    private MainSceneUIManager mainSceneUIManager;

    private void Start()
    {
        mainSceneUIManager = GameObject.Find("MainSceneUIManager").GetComponent<MainSceneUIManager>();
        closeButton.onClick.AddListener(Close);
    }

    private void Close()
    {
        mainSceneUIManager.SetLoginPanelVisible(true);
        Destroy(gameObject);
    }
}
