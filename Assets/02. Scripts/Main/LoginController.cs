using System;
using TMPro;
using UnityEngine;

public class LoginController : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordInputField;

    private AuthenticationManager _authenticationManager;

    private void Start()
    {
        _authenticationManager = GameObject.Find("Authentication Manager").GetComponent<AuthenticationManager>();
    }

    public void OnClickLoginButton()
    {
        string username = emailField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            // 아이디 비번중 하나가 비었을때
            GameManager.Instance.OpenConfirmPanel("아이디 또는 비밀번호를 입력해주세요.", () =>
            {
                emailField.text = "";
                passwordInputField.text = "";
            });
            return;
        }

        // // 아직 네트워크 연결 없으니 -> 무조건 실패처리
        // GameManager.Instance.OpenConfirmPanel("아이디가 존재하지 않습니다.", () =>
        // {
        //     emailField.text = "";
        //     passwordInputField.text = "";
        // });

        _authenticationManager.username = username;
        _authenticationManager.password = password;
        _authenticationManager.emailField = emailField;
        _authenticationManager.passwordInputField = passwordInputField;

        _authenticationManager.OnLoginButtonClicked();
    }

    public void OnClickSignupButton()
    {
        // 나중에 회원가입 패널 연결 개발 시 추가
    }
}