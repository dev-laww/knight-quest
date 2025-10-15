using System.Linq;
using Game.Autoloads;
using Game.Data;
using Godot;
using GodotUtilities;

namespace Game.UI;

[Scene]
public partial class LevelSelect : CanvasLayer
{
    [Node] private ResourcePreloader resourcePreloader;
    [Node] public GridContainer levelsContainer;

    private PackedScene getCertScene = GD.Load<PackedScene>("res://scenes/ui/overlays/get_cert.tscn");

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }

    public override void _Ready()
    {
        var finishedLevels = SaveManager.LoadFinishedLevels().Select(l => l.Id).ToHashSet();
        var subject = GameManager.Config.Subject;
        var grade = GameManager.Config.Grade;
        var levels = LevelRegistry.GetLevels();

        foreach (var info in levels)
        {
            if (info.Subject == subject && info.Grade == grade)
            {
                var level = resourcePreloader.InstanceSceneOrNull<Level>();
                if (level == null) return;

                level.Setup(info);
                levelsContainer.AddChild(level);
                level.Pressed += () => OnLevelPressed(info);

                // Disable if finished
                if (!finishedLevels.Contains(info.ResourcePath)) continue;

                level.DisableLevel();
            }
        }

        if (!AreAllLevelsFinished()) return;
        ShowScreen();
    }

    private void ShowScreen()
    {
        var screen = getCertScene.Instantiate();
        GetTree().Root.AddChild(screen);
        GD.Print("All levels are finished!");
    }

    private static bool AreAllLevelsFinished()
    {
        var allLevelIds = LevelRegistry.PublicResources.Keys.ToHashSet();
        var finishedLevelIds = SaveManager.LoadFinishedLevels().Select(l => l.Id).ToHashSet();
        return allLevelIds.All(id => finishedLevelIds.Contains(id));
    }

    private static void OnLevelPressed(LevelInfo level)
    {
        AudioManager.Instance.PlayClick();
        GameManager.SetLevel(level);
        QuestionManager.Instance.LoadQuestions(level.Questions);
        Navigator.Push("res://scenes/world/battle.tscn");
    }
}