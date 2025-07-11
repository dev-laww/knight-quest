using Game.Data;
using Godot;
using GodotUtilities;

namespace Game.UI;

[Scene]
public partial class LevelSelect : CanvasLayer
{
    [Export] private LevelInfo[] levels = [];

    [Node] private ResourcePreloader resourcePreloader;

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }
}