using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using TMPro;

public class AuthenticationManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField nicknameInput; // 닉네임 입력 필드 추가
    public TextMeshProUGUI resultText;
    public Button signupButton;
    public Button loginButton;

    private string baseUrl = "http://localhost:3000/api/";

    void Start()
    {
        signupButton.onClick.AddListener(OnSignupButtonClicked);
        loginButton.onClick.AddListener(OnLoginButtonClicked);
    }

    void OnSignupButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        string nickname = nicknameInput.text; // 닉네임 값 가져오기

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(nickname))
        {
            resultText.text = "모든 정보를 입력해주세요.";
            return;
        }
        StartCoroutine(SendAuthRequest("signup", username, password, nickname));
    }

    void OnLoginButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        StartCoroutine(SendAuthRequest("login", username, password));
    }

    // 서버에 HTTP 요청을 보내는 코루틴
    IEnumerator SendAuthRequest(string endpoint, string username, string password, string nickname = null)
    {
        string json;
        if (endpoint == "signup")
        {
            // 회원가입 요청 시 닉네임 추가
            json = "{\"username\":\"" + username + "\", \"password\":\"" + password + "\", \"nickname\":\"" + nickname + "\"}";
        }
        else
        {
            // 로그인 요청 시 닉네임 제외
            json = "{\"username\":\"" + username + "\", \"password\":\"" + password + "\"}";
        }

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest www = new UnityWebRequest(baseUrl + endpoint, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                resultText.text = "네트워크 오류: " + www.error;
            }
            else
            {
                string responseText = www.downloadHandler.text;
                AuthResponse authResponse = JsonUtility.FromJson<AuthResponse>(responseText);
                resultText.text = authResponse.message;

                if (endpoint == "login" && authResponse.success)
                {
                    Debug.Log("로그인 성공! 게임 시작 화면으로 전환합니다.");
                }
            }
        }
    }

    [System.Serializable]
    private class AuthResponse
    {
        public bool success;
        public string message;
    }
}