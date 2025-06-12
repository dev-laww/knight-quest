using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Login : MonoBehaviour
{
    [SerializeField] private UIDocument document;

    private Button loginButton;
    private Button googleLoginButton;
    private TextField emailField;
    private TextField passwordField;

    private void Awake()
    {
        Application.deepLinkActivated += OnDeepLinkActivated;
    }

    private void Start()
    {
        var root = document.rootVisualElement;
        loginButton = root.Q<Button>("login");
        googleLoginButton = root.Q<Button>("google-login");
        emailField = root.Q<TextField>("email");
        passwordField = root.Q<TextField>("password");

        loginButton.clicked += OnLoginButtonClicked;
        googleLoginButton.clicked += () => Application.OpenURL($"{Constants.API_URL}/auth/google/login");
    }


    private void OnLoginButtonClicked()
    {
        var email = emailField.value;
        var password = passwordField.value;

        // Implement login logic here
        Debug.Log($"Login attempted with Email: {email}, Password: {password}");
    }

    private void OnDeepLinkActivated(string url)
    {
        Debug.Log($"Deep link activated: {url}");

        DeepLinkManager.Instance.Parameters.TryGetValue("authToken", out var authToken);

        Debug.Log(authToken);
    }
}