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

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }

    public override void _Ready()
    {
        foreach (var levelInfo in levels)
        {
            var level = resourcePreloader.InstanceSceneOrNull<Level>();

            if (level == null) return;

            level.LevelInfo = levelInfo;
            levelsContainer.AddChild(level);
        }
    }
}