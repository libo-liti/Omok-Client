using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MutiplayRoomPrefab : MonoBehaviour
{
    [SerializeField] private Button enterButton;
    public TextMeshProUGUI roomName;
    public TextMeshProUGUI mode;

    private void Start()
    {
        enterButton.onClick.AddListener(() =>
        {
            NetworkManager.Instance.JoinRoomCheck(roomName.text);
        }); 
    }
}