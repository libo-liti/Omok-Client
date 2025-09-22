using System;
using Michsky.MUIP;
using UnityEngine;
using UnityEngine.UI;

public class MultiRoomList : MonoBehaviour
{
    [SerializeField] private Transform content;
    // [SerializeField] private Button closedButton;
    [SerializeField] private ButtonManager closedButton;
    [SerializeField] private Button refreshButton;
    
    private void OnEnable()
    {
        NetworkManager.Instance.roomListContent = content;
        closedButton.onClick.AddListener(NetworkManager.Instance.Dispose);
        refreshButton.onClick.AddListener(NetworkManager.Instance.GetRooms);
    }
}
