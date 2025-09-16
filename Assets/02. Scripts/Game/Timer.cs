using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshPro timerText;   // 3D TMP 전용

    [SerializeField] private float timeLimit = 30f;     // 기본 시간
    [SerializeField] private UnityEvent onTimerEnd;     // 시간이 끝났을 때 실행할 이벤트

    private float timeRemaining;
    private bool isRunning;

    private void Awake()
    {
        // Inspector에서 안 넣어줬다면 자식에서 TMP 찾기
        if (timerText == null)
            timerText = GetComponentInChildren<TextMeshPro>();
    }

    private void Start()
    {
        StopTimer(); // 시작 시 숨기기
    }
    
    public void StartTimer(bool first=true)
    {
        Debug.Log("StartTimer");
        timeRemaining = timeLimit;
        isRunning = true;
        gameObject.SetActive(true);
        if (first)
        {
            timerText.color = Color.black;
        }
        else
        {
            timerText.color = Color.white;
        }
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