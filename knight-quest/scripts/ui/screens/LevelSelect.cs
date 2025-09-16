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
    [Node] private GridContainer levelsContainer;
    [Node] private Button backButton;

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }

    public override void _Ready()
    {
        backButton.Pressed += () => Navigator.Back();
        foreach (var levelInfo in levels)
        {
            var level = resourcePreloader.InstanceSceneOrNull<Level>();

            if (level == null) return;

            level.Setup(levelInfo);
            levelsContainer.AddChild(level);
            level.Pressed += () => OnLevelPressed(levelInfo);
        }
    }

    private void OnLevelPressed(LevelInfo level)
    {
        GameManager.SetLevel(level);
        QuestionManager.Instance.LoadQuestions(level.Questions);
        Navigator.Push("res://scenes/world/battle.tscn");
    }
}