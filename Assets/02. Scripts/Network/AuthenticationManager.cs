using UnityEngine;
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
    public string nickname;
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
    public MainSceneUIManager mainSceneUIManager;
    
    public TMP_InputField emailField;
    public TMP_InputField passwordInputField;
    
    public string username;
    public string password;
    public string nickname;
    
    public void OnSignupButtonClicked()
    {
        StartCoroutine(SendAuthRequest("api/signup", username, password, nickname));
    }

    public void OnLoginButtonClicked()
    {
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
            
            // 네트워크 접속
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("네트워크 성공");
                if (endpoint == "api/login" && authResponse.success)
                {
                    GameManager.Instance.guestName = authResponse.nickname;
                    mainSceneUIManager.RefreshUI();
                }
            }
            else
            {
                Debug.Log("네트워크 실패");
                if (endpoint == "api/login")
                {
                    if (authResponse.result == "id")
                    {
                        GameManager.Instance.OpenConfirmPanel("아이디가 일치하지 않습니다.", () =>
                        {
                            emailField.text = "";
                            passwordInputField.text = "";
                        });
                    }
                    else if (authResponse.result == "password")
                    {
                        GameManager.Instance.OpenConfirmPanel("비밀번호가 일치하지 않습니다.", () =>
                        {
                            passwordInputField.text = "";
                        });
                    }
                    else
                        Debug.Log("로그인 실패 : " + www.error);
                }
                else if (endpoint == "api/signup")
                {
                    Debug.Log("회원가입 실패");
                    Debug.Log(authResponse.message);
                    if (authResponse.result == "id")
                    {
                        Debug.Log("회원가입 아이디");
                        GameManager.Instance.OpenConfirmPanel("해당 아이디가 이미 있습니다.", () =>
                        {
                            Debug.Log("아이디 중복");
                        });
                    }
                }
            }
        }
    }

}