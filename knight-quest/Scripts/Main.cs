using Godot;
using GodotUtilities;

namespace Game;

[Scene]
public partial class Main : Node
{
    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }
}
