using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AskPanelController : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button denyButton;

    private System.Action _onYes;
    private System.Action _onNo;

    public void Show(string message, System.Action onYes, System.Action onNo)
    {
        messageText.text = message;
        _onYes = onYes;
        _onNo = onNo;
        
        acceptButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(() =>
        {
            _onYes?.Invoke();
            Destroy(gameObject);
        });
        
        denyButton.onClick.RemoveAllListeners();
        denyButton.onClick.AddListener(() =>
        {
            _onNo?.Invoke();
            Destroy(gameObject);
        });
    }
}