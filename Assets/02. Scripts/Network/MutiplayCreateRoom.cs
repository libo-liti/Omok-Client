using System;
using TMPro;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MutiplayCreateRoom : MonoBehaviour
{
    [SerializeField] private TMP_InputField roomNameInputField;
    [FormerlySerializedAs("nomal")] [SerializeField] private Toggle normal;
    [SerializeField] private Toggle arcade;
    [SerializeField] private Button createButton;

    private void Start()
    {
        normal.isOn = true;
        arcade.isOn = false;

        normal.onValueChanged.AddListener((isCheck) =>
        {
            if (isCheck)
            {
                normal.isOn = true;
                arcade.isOn = false;
            }
        });

        arcade.onValueChanged.AddListener((isCheck) =>
        {
            if (isCheck)
            {
                normal.isOn = false;
                arcade.isOn = true;
            }
        });
        
        createButton.onClick.AddListener(OnClickedButton);
    }

    private void OnClickedButton()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            GameManager.Instance.OpenConfirmPanel("방 이름을 입력해 주세요", null);
            return;
        }

        var mode = normal.isOn ? "normal" : "arcade";
        
        NetworkManager.Instance.CreateRoomCheck(roomNameInputField.text, mode);
    }
}
