using Godot;
using System;
using Game.Autoloads;
using GodotUtilities;
using GodotUtilities.Util;


namespace Game.UI;

[Scene]
public partial class ResultScreen : CanvasLayer
{

    
    [Node] private RichTextLabel resultLabel;
    [Node] private Label rewardsLabel;
    [Node] private Button menuButton;
    [Node] private Button retryButton;
    


    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }

    public override void _Ready()
    {
        menuButton.Pressed += OnMenuButtonPress;
        retryButton.Pressed += OnRetryButtonPress;
        
    }

    private void OnMenuButtonPress()
    {

        Navigator.Push("res://scenes/ui/screens/main_menu.tscn");
        QueueFree();
    }

    private void OnRetryButtonPress()
    {
        Navigator.Push("res://scenes/ui/overlays/result_screen.tscn");
        QueueFree();
    }



public void ShowResult(bool victory, int starsEarned = 0)
{
    if (victory)
    {
        resultLabel.Text = "Victory!";
        resultLabel.AddThemeColorOverride("font_color", Colors.Green);
        rewardsLabel.Text = $"Stars earned: {starsEarned}";
    }
    else
    {
        resultLabel.Text = "Defeat...";
        resultLabel.AddThemeColorOverride("font_color", Colors.Red);
        rewardsLabel.Text = "No stars earned.";
    }
}
    }
