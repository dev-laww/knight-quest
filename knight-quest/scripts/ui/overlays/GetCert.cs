using Godot;
using System;
using GodotUtilities;

namespace Game.UI;
[Scene]
public partial class GetCert : CanvasLayer
{
    [Node] private Button getCertBtn;
    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }

    public override void _Ready()
    {
    
    }
}
