using Godot;
using System;
using Game.Autoloads;
using GodotUtilities;

namespace Game.UI;

[Scene]
public partial class Login : CanvasLayer
{
    [Node] private LineEdit usernameField;
    [Node] private LineEdit passwordField;
    [Node] private Button loginButton;
    [Node] private Button registerButton;

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }

    public override void _Ready()
    {
        loginButton.Pressed += OnLoginPressed;
        registerButton.Pressed += OnRegisterPressed;
    }

    private void OnLoginPressed()
    {
        var username = usernameField.Text;
        var password = passwordField.Text;

        if (SaveManager.Login(username, password))
        {
            GD.Print("Welcome " + username + "!");
            Navigator.Push("res://scenes/ui/screens/main_menu.tscn");
        }
        else
        {
            GD.Print("Login failed!");
        }
    }

    private void OnRegisterPressed()
    {
        var username = usernameField.Text;
        var password = passwordField.Text;

        if (SaveManager.Register(username, password))
        {
            GD.Print("Account created!");
        }
        else
        {
            GD.Print("Account already exists!");
        }
    }
}