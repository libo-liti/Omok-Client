using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;   // 3D TMP 전용
    [SerializeField] private Image timerBackgroundImage; //타이머 배경
    [SerializeField] private float timeLimit = 30f;     // 기본 시간
    private UnityAction onTimerEnd;     // 시간이 끝났을 때 실행할 이벤트

    private float timeRemaining;
    private bool isRunning;

    private void Awake()
    {
        // Inspector에서 안 넣어줬다면 자식에서 TMP 찾기
        if (timerText == null)
            timerText = GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
    }
    
    public void StartTimer(bool first=true, UnityAction callback=null)
    {
        onTimerEnd = callback;
        timeRemaining = timeLimit;
        isRunning = true;
        gameObject.SetActive(true);
        timerText.color = first? Color.white:Color.black;
        timerBackgroundImage.color = first? new Color(0f, 0f, 0f, 0.6f) : new Color(1f, 1f, 1f, 0.6f);
            
    }
    
    public void StopTimer()
    {
        isRunning = false;
        if (timerText != null) timerText.text = "";
        gameObject.SetActive(false);
        
    }

    private void Update()
    {
        if (!isRunning) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isRunning = false;

            if (timerText)
                timerText.text = "0";

            // Inspector에서 연결한 함수들 실행
            onTimerEnd?.Invoke();
        }
        else
        {
            if (timerText)
                timerText.text = Mathf.CeilToInt(timeRemaining).ToString();
        }
    }
}