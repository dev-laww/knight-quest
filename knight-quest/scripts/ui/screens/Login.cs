using Godot;
using Game.Autoloads;
using Game.Utils;
using GodotUtilities;
using Logger = Game.Utils.Logger;

namespace Game.UI;

[Scene]
public partial class Login : CanvasLayer
{
    [Node] private LineEdit usernameField;
    [Node] private LineEdit passwordField;
    [Node] private Button loginButton;
    [Node] private Button registerButton;
    [Node] private Button googleButton;
    [Node] private Node deeplink;

    [Export] private string googleAuthUrl;

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }

    public override void _Ready()
    {
        loginButton.Pressed += OnLoginPressed;
        registerButton.Pressed += OnRegisterPressed;
        googleButton.Pressed += OnGooglePressed;

        deeplink.Call("initialize");
    }

    private void OnDeepLinkReceived(RefCounted variant)
    {
        var link = new DeepLinkUrl(variant);
        Logger.Debug($"Deep link received: {link.GetData()}");
    }

    private void OnLoginPressed()
    {
        // var username = usernameField.Text;
        // var password = passwordField.Text;
        AudioManager.Instance.PlayClick();
        Navigator.Push("res://scenes/ui/screens/main_menu.tscn");
    }

    private void OnRegisterPressed()
    {
        AudioManager.Instance.PlayClick();
        Navigator.Push("res://scenes/ui/screens/register.tscn");
    }

    private void OnGooglePressed()
    {
        AudioManager.Instance.PlayClick();
        OS.ShellOpen(googleAuthUrl);
    }
}