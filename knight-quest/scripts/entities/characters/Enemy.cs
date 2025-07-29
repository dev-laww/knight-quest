using Godot;
using System;
using System.Threading.Tasks;
using GodotUtilities;


namespace Game.Entities;
[Scene]
public partial class Enemy : Entity
{
    private const string ATTACK = "attack";
    private const string HURT = "hurt";

    [Node] private AnimationTree animationTree;

    private AnimationNodeStateMachinePlayback playback;

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }

    public override void _Ready()
    {
        this.AddToGroup();
        playback = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
    }

    public override async Task TakeTurn(Entity target)
    {
        await base.TakeTurn(target);
        playback.Travel(ATTACK);

        await ToSignal(animationTree, "animation_finished");
    }
}
