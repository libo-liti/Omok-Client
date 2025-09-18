using System;
using UnityEngine;
using UnityEngine.UI;

public class Emoji : MonoBehaviour
{
    [SerializeField] private int _n;
    private Action<int> _touchedEmoji;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnTouchedEmoji);
    }

    public void Init(int n, Action<int> touchedEmoji)
    {
        _n = n;
        _touchedEmoji = touchedEmoji;
    }

    private void OnTouchedEmoji()
    {
        _touchedEmoji?.Invoke(_n);
    }
}
