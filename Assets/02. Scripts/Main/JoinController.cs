using System;
using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;

public class JoinController : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField nicknameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_InputField confirmPasswordField;

    [SerializeField] private AuthenticationManager _authenticationManager;
        
    private void Start()
    {
        _authenticationManager = GameObject.Find("NetworkManager").GetComponent<AuthenticationManager>();
    }

    public void OnClickJoinButton()
    {
        string email = emailField.text.Trim();
        string nickname = nicknameField.text.Trim();
        string password = passwordField.text;
        string confirmPassword = confirmPasswordField.text;
        
        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.(com|co\.kr)$"))
        {
            GameManager.Instance.OpenConfirmPanel("이메일 형식이 올바르지 않습니다. \n(.com 또는 co.kr 형식인지 확인해 주세요)", null);
            return;
        }
        
        if (nickname.Length == 0 || nickname.Length > 8)
        {
            GameManager.Instance.OpenConfirmPanel("닉네임은 1~8글자 이내로 입력해주세요.", null);
            return;
        }
        
        if (password.Length == 0 || password.Length > 12)
        {
            GameManager.Instance.OpenConfirmPanel("비밀번호는 최대 12자리까지 가능합니다.", null);
            return;
        }
        
        if (password != confirmPassword)
        {
            GameManager.Instance.OpenConfirmPanel("비밀번호가 일치하지 않습니다.", null);
            return;
        }
        
        GameManager.Instance.OpenConfirmPanel("회원가입이 완료되었습니다!", () =>
        {
            emailField.text = "";
            nicknameField.text = "";
            passwordField.text = "";
            confirmPasswordField.text = "";
        });
        if (_authenticationManager != null)
        {
            _authenticationManager.username = email;
            _authenticationManager.password = password;
            _authenticationManager.nickname = nickname;
        }
        
        _authenticationManager.OnSignupButtonClicked();
        
        Destroy(gameObject);
    }

    public void OnClickCancelButton()
    {
        emailField.text = "";
        nicknameField.text = "";
        passwordField.text = "";
        confirmPasswordField.text = "";
    }
}