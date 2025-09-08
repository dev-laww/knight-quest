using Game.Data;
using Game.Utils;
using Godot;
using GodotUtilities;

namespace Game;

[Scene]
public partial class CharacterPanel : Panel
{
    [Export] public Character character;

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }

    public override void _Ready()
    {
        Callable.From(() =>
        {
            var rect = GetRect();
            PivotOffset = rect.Size / 2;

            Logger.Debug(rect);

            var instance = character.Scene.InstantiateOrNull<Node2D>();

            if (instance == null) return;

            AddChild(instance);
            instance.Position = PivotOffset;
        }).CallDeferred();
    }
}