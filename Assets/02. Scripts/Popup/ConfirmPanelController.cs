using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPanelController : MonoBehaviour
{
    [SerializeField] 
    private TMP_Text messageText;
    [SerializeField] 
    private Button confirmButton;

    private System.Action _onConfirm;

    public void Show(string message, System.Action onConfirm)
    {
        messageText.text = message;
        _onConfirm = onConfirm;

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            _onConfirm?.Invoke();
            Destroy(gameObject); 
        });
    }
}