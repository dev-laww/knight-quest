using UnityEngine;

public class Main : MonoBehaviour
{
    public void OnLoginButtonPressed()
    {
        Debug.Log("Login button pressed");
        // Here you can add the logic to handle the login process

        Application.OpenURL("https://google.com");
    }
}