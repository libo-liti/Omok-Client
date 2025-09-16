using UnityEngine;
using UnityEngine.UI;

public class MenuPanelController : MonoBehaviour
{
    [SerializeField] 
    private Button closeButton;

    private void Start()
    {
        closeButton.onClick.AddListener(Close);
    }

    private void Close()
    {
        Destroy(gameObject);
    }
}
