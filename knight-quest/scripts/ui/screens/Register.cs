using Game.Autoloads;
using Godot;
using GodotUtilities;
using Logger = Game.Utils.Logger;

namespace Game;

[Scene]
public partial class Register : CanvasLayer
{
    [Node] private LineEdit firstName;
    [Node] private LineEdit lastName;
    [Node] private LineEdit username;
    [Node] private LineEdit password;
    [Node] private LineEdit confirmPassword;
    [Node] private Button registerButton;

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }

    public override void _Ready()
    {
        registerButton.Pressed += OnRegisterButtonPressed;
    }

    private async void OnRegisterButtonPressed()
    {
        if (password.Text != confirmPassword.Text) return;

        var body = new
        {
            firstName = firstName.Text,
            lastName = lastName.Text,
            username = username.Text,
            password = password.Text
        };

        var res = await ApiClient.PostAsync("/auth/register", body);
        
        Logger.Debug(res);
    }   
}