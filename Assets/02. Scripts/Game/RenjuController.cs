using UnityEngine;

public class RenjuController : MonoBehaviour
{
    private SpriteRenderer[] _xSprites;

    /// <summary>
    /// 모든 금수 표시를 초기화
    /// </summary>
    public void Init()
    {
        _xSprites = GetComponentsInChildren<SpriteRenderer>();

        HideAll();
    }
    
    /// <summary>
    /// 특정 좌표의 금수 표시 활성화
    /// </summary>
    public void ShowX(int row, int col)
    {
        // row, col >> index 변환
        var pointIndex = row * Constants.BoardSize + col;

        // 금수 표시
        _xSprites[pointIndex].gameObject.SetActive(true);
    }

    /// <summary>
    /// 모든 금수 표시 비활성화
    /// </summary>
    public void HideAll()
    {
        for (int i = 0; i < _xSprites.Length; i++)
        {
            _xSprites[i].gameObject.SetActive(false);
        }
    }
}
