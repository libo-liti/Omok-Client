using UnityEngine;
public class GameModeSelectManager : MonoBehaviour
{
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
    
    public void OnClickArcadePlay()
    {
        GameManager.Instance.ChangeToGameScene(Constants.GameType.ArcadePlay);
    }
}

