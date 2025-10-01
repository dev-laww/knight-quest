using System.Linq;
using Game.Autoloads;
using Game.Data;
using Godot;
using GodotUtilities;

namespace Game.UI;

[Scene]
public partial class LevelSelect : CanvasLayer
{
    [Export] private LevelInfo[] levels = [];

    [Node] private ResourcePreloader resourcePreloader;
    [Node] public GridContainer levelsContainer;

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
                if (finishedLevels.Contains(info.LevelName))
                {
                    level.DisableLevel();
                }
            }
        }
    }

    private void OnLevelPressed(LevelInfo level)
    {
        GameManager.SetLevel(level);
        QuestionManager.Instance.LoadQuestions(level.Questions);
        Navigator.Push("res://scenes/world/battle.tscn");
    }
}