using Godot;
using GodotUtilities;

namespace Game.Entities;

[Scene]
public partial class Player : Entity
{
    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }

    public override void _Ready()
    {
        this.AddToGroup();
    }
}