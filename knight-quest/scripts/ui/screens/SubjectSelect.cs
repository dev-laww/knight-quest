using Game.Autoloads;
using Game.Data;
using Godot;
using GodotUtilities;

namespace Game;

[Scene]
public partial class SubjectSelect : CanvasLayer
{
    [Node] private Panel math;
    [Node] private Panel english;

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }

    public override void _Ready()
    {
        math.GuiInput += e => OnSubjectGuiInput(e, RunConfig.SubjectArea.Mathematics);
        english.GuiInput += e => OnSubjectGuiInput(e, RunConfig.SubjectArea.English);
    }

    private void OnSubjectGuiInput(InputEvent @event, RunConfig.SubjectArea subject)
    {
        if (@event is not InputEventMouseButton mouseAction) return;
        if (!mouseAction.Pressed || mouseAction.ButtonIndex != MouseButton.Left) return;

        GameManager.SetSubjectArea(subject);
        GetTree().ChangeSceneToFile("res://scenes/ui/screens/character_select.tscn");
    }
}