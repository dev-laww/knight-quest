using Game.Data;
using Godot;
using GodotUtilities;

namespace Game;

[Scene]
public partial class GradeLevelPanel : Control
{
    public RunConfig.GradeLevel GradeLevel { get; set; }

    [Node] public Panel Panel;

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }
}