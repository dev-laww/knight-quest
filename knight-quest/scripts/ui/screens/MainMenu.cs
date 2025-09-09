using Game.Autoloads;
using Godot;
using GodotUtilities;

namespace Game;

[Scene]
public partial class MainMenu : CanvasLayer
{
    [Node] private Button startButton;

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }

    public override void _Ready()
    {
        startButton.Pressed += OnStartButtonPressed;
    }

    private void OnStartButtonPressed()
    {
        Navigator.Push("res://scenes/ui/screens/subject_select.tscn");
    }
}