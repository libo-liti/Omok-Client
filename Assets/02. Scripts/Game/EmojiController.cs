using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


public class EmojiController : MonoBehaviour
{
    [SerializeField] private GameObject emojiPanel;
    [SerializeField] private Button emojiButton;
    public Action<int> EmitEmoji;
    
    public Sprite[] sprites;
    private Emoji[] _emojis;
    
    public Image myEmoji;
    public Image opponentEmoji;
    
    private RectTransform _myEmojiRect;
    private RectTransform _opponentEmojiRect;
    
    public void Init()
    {
        emojiButton.onClick.AddListener(EmojiPanelToggle);
        _emojis = GetComponentsInChildren<Emoji>();

        _opponentEmojiRect = opponentEmoji.GetComponent<RectTransform>();
        _myEmojiRect = myEmoji.GetComponent<RectTransform>();
        EmojiPanelToggle();

        for (int i = 0; i < _emojis.Length; i++)
        {
            _emojis[i].GetComponent<Button>().image.sprite = sprites[i];
            _emojis[i].Init(i, (n) =>
            {
                SetEmoji(n, true);
                EmitEmoji?.Invoke(n);
                EmojiPanelToggle();
            });
        }
    }

    public void SetEmoji(int n, bool isMine)
    {
        if (isMine)
        {
            myEmoji.sprite = sprites[n];
            EmojiMotion(_myEmojiRect);
        }
        else
        {
            opponentEmoji.sprite = sprites[n];
            EmojiMotion(_opponentEmojiRect);
        }
    }

    private void EmojiMotion(RectTransform rect)
    {
        // 이모티콘을 처음에 보이지 않게 설정
        rect.localScale = Vector3.zero;
        // DOTween 시퀀스 생성
        Sequence sequence = DOTween.Sequence();
        // 1. 이모티콘을 커지게 하는 트윈 (0.5초 동안 크기 1.5배로)
        sequence.Append(rect.DOScale(1.5f, 0.5f).SetEase(Ease.OutBack));
        // 2. 잠시 대기 (0.5초)
        sequence.AppendInterval(0.5f);
        // 3. 이모티콘을 다시 작아지게 하는 트윈 (0.3초 동안 크기 0으로)
        sequence.Append(rect.DOScale(0, 0.3f).SetEase(Ease.InBack));
        // 시퀀스 재생
        sequence.Play();
    }
    
    private void EmojiPanelToggle()
    {
        emojiPanel.SetActive(!emojiPanel.activeSelf);
    }

    public void OnSelectEmoji(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            var key = context.control.name;
            int value;
            switch (key)
            {
                case "f1" : value = 10; break;
                case "f2" : value = 11; break;
                case "f3" : value = 12; break;
                case "f4" : value = 13; break;
                case "f5" : value = 14; break;
                case "f6" : value = 15; break;
                case "f7" : value = 16; break;
                case "f8" : value = 17; break;
                case "f9" : value = 18; break;
                case "f10" : value = 19; break;
                default:
                    value = int.Parse(context.control.name);
                    break;
            }
            
            SetEmoji(value, true);
            EmitEmoji?.Invoke(value);
        }
    }
}
