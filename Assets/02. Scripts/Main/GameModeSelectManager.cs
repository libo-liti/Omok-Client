using System;
using UnityEngine;
using UnityEngine.UI;

public class GameModeSelectManager : MonoBehaviour
{
    [SerializeField] private Button multiplayButton;

    private void OnEnable()
    {
        multiplayButton.onClick.AddListener(NetworkManager.Instance.Init);
    }

    public void OnClickSinglePlay()
    {
        GameManager.Instance.ChangeToGameScene(Constants.GameType.SinglePlay);
    }

    public void OnClickDualPlay()
    {
        GameManager.Instance.ChangeToGameScene(Constants.GameType.DualPlay);
    }

    public void OnClickMultiPlay()
    {
        GameManager.Instance.ChangeToGameScene(Constants.GameType.MultiPlay);
    }
}

