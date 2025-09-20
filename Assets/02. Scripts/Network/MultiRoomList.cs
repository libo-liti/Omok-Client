using System;
using UnityEngine;
using UnityEngine.UI;

public class MultiRoomList : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private Button closedButton;

    private void OnEnable()
    {
        NetworkManager.Instance.roomListContent = content;
        closedButton.onClick.AddListener(NetworkManager.Instance.Dispose);
    }
}
