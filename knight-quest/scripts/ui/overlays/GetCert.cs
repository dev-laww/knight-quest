using Godot;
using System;
using Game.Autoloads;
using GodotUtilities;

namespace Game.UI;
[Scene]
public partial class GetCert : CanvasLayer
{
    [Node] private Button getCertBtn;
    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }

    public override void _Ready()
    {
        getCertBtn.Pressed += OnOpenLinkButtonPressed;
    }
    public void OnOpenLinkButtonPressed()
    {
        AudioManager.Instance.PlayClick();
        string url = ""; 
        OpenLink(url);
    }

    private void OpenLink(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            GD.PrintErr("URL is empty!");
            return;
        }

        if (OS.HasFeature("web"))
        {
            JavaScriptBridge.Eval($"window.open('{url}', '_blank')");
        }
        else
        {
            OS.ShellOpen(url);
        }
    }
}
