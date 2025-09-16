using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using TMPro;

[System.Serializable]
public class AuthResponse
{
    public bool success;
    public string message;
    public string result;
}

[System.Serializable]
public class SignupData
{
    public string username;
    public string password;
    public string nickname;
}

[System.Serializable]
public class SigninData
{
    public string username;
    public string password;
}

public class AuthenticationManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField passwordCheckInput;
    public TMP_InputField nicknameInput; // 닉네임 입력 필드 추가
    public TextMeshProUGUI resultText;
    public Button signupButton;
    public Button loginButton;

    void Start()
    {
        signupButton.onClick.AddListener(OnSignupButtonClicked);
        loginButton.onClick.AddListener(OnLoginButtonClicked);
    }

    void OnSignupButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        string passwordCheck = passwordCheckInput.text;
        string nickname = nicknameInput.text; // 닉네임 값 가져오기

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordCheck)
            || string.IsNullOrEmpty(nickname))
        {
            resultText.text = "모든 정보를 입력해주세요.";
            return;
        }
        else if (password != passwordCheck)
        {
            resultText.text = "비밀번호가 일치하지 않습니다.";
            return;
        }
        
        StartCoroutine(SendAuthRequest("api/signup", username, password, nickname));
    }

    void OnLoginButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            resultText.text = "모든 정보를 입력해주세요.";
            return;
        }
        
        StartCoroutine(SendAuthRequest("api/login", username, password));
    }

    // 서버에 HTTP 요청을 보내는 코루틴
    IEnumerator SendAuthRequest(string endpoint, string username, string password, string nickname = null)
    {
        string json;
        if (endpoint == "api/signup")
        {
            SignupData data = new SignupData();
            data.username = username;
            data.password = password;
            data.nickname = nickname;
            
            // struct -> json
            // {"username":"user123", "password":"pass456", "nickname":"tester"}
            json = JsonUtility.ToJson(data); // JsonUtility를 사용하여 JSON 문자열로 변환
            
            // 구조체 말고 문자열로도 가능
            // json = "{\"username\":\"" + username + "\", \"password\":\"" + password + "\", \"nickname\":\"" + nickname + "\"}";
        }
        else
        {
            SigninData data = new SigninData();
            data.username = username;
            data.password = password;
            json = JsonUtility.ToJson(data); // JsonUtility를 사용하여 JSON 문자열로 변환
               
            // json = "{\"username\":\"" + username + "\", \"password\":\"" + password + "\"}";
        }

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + endpoint, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            string responseText = www.downloadHandler.text;
            AuthResponse authResponse = JsonUtility.FromJson<AuthResponse>(responseText);
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                resultText.text = authResponse.message;
            }
            else
            {
                resultText.text = authResponse.message;

                if (endpoint == "login" && authResponse.success)
                {
                    Debug.Log("로그인 성공! 게임 시작 화면으로 전환합니다.");
                }
            }
        }
    }

}