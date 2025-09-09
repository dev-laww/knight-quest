using System;
using Game.Autoloads;
using Game.Data;
using Godot;
using GodotUtilities;

namespace Game;

[Scene]
public partial class GradeLevelSelect : CanvasLayer
{
    [Node] private HBoxContainer gradeLevelsContainer;
    [Node] private ResourcePreloader resourcePreloader;

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }

    public override void _Ready()
    {
        foreach (var gradeLevel in Enum.GetValues<RunConfig.GradeLevel>())
        {
            var panel = resourcePreloader.InstanceSceneOrNull<GradeLevelPanel>("GradeLevelPanel");

            if (panel == null) continue;

            panel.GradeLevel = gradeLevel;
            gradeLevelsContainer.AddChild(panel);
            panel.Panel.GuiInput += e => OnGradeLevelPanelGuiInput(e, gradeLevel);
        }
    }

    private void OnGradeLevelPanelGuiInput(InputEvent @event, RunConfig.GradeLevel gradeLevel)
    {
        var pressed = @event switch
        {
            InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left } => true,
            InputEventScreenTouch { Pressed: true } => true,
            _ => false
        };

        if (!pressed) return;

        GameManager.SetGradeLevel(gradeLevel);
        Navigator.Push("res://scenes/ui/screens/level_select.tscn");
    }
}