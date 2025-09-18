using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EmojiController : MonoBehaviour
{
    [SerializeField] private GameObject emojiPanel;
    [SerializeField] private Button emojiButton;
    public Action<int> EmitEmoji;
    
    [SerializeField] private Emoji[] _emojis;
    public Sprite[] sprites;
    public Image img;
    private RectTransform emoticonRect;
    
    public void Init()
    {
        emojiButton.onClick.AddListener(EmojiPanelToggle);
        _emojis = GetComponentsInChildren<Emoji>();
        emoticonRect = img.GetComponent<RectTransform>();
        EmojiPanelToggle();

        for (int i = 0; i < _emojis.Length; i++)
        {
            _emojis[i].Init(i, (n) =>
            {
                SetEmoji(n);
                EmitEmoji?.Invoke(n);
                EmojiPanelToggle();
            });
        }
    }

    public void SetEmoji(int n)
    {
        img.sprite = sprites[n];
        
        // 이모티콘을 처음에 보이지 않게 설정
        emoticonRect.localScale = Vector3.zero;
        
        // DOTween 시퀀스 생성
        Sequence sequence = DOTween.Sequence();
        
        // 1. 이모티콘을 커지게 하는 트윈 (0.5초 동안 크기 1.5배로)
        sequence.Append(emoticonRect.DOScale(1.5f, 0.5f).SetEase(Ease.OutBack));
        
        // 2. 잠시 대기 (0.5초)
        sequence.AppendInterval(0.5f);
        
        // 3. 이모티콘을 다시 작아지게 하는 트윈 (0.3초 동안 크기 0으로)
        sequence.Append(emoticonRect.DOScale(0, 0.3f).SetEase(Ease.InBack));

        // 시퀀스 재생
        sequence.Play();
    }
    
    private void EmojiPanelToggle()
    {
        emojiPanel.SetActive(!emojiPanel.activeSelf);
    }
}
