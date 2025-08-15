using System.Threading.Tasks;
using Godot;
using System.Collections.Generic;
using Game.Components;
using Game.Data;
using Game.Utils;
using GodotUtilities;

namespace Game.Entities;

// TODO: make use of a state machine for the player
[Scene]
public partial class Player : Entity
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

        StatsManager.StatDecreased += OnStatDecreased;
    }

    private void OnStatDecreased(int amount, StatsManager.Stat stat)
    {
        if (stat != StatsManager.Stat.Health) return;

        playback.Travel(HURT);
    }

    public override async Task TakeTurn(Entity target)
    {
        await base.TakeTurn(target);
        playback.Travel(ATTACK);
        await ToSignal(animationTree, "animation_finished");
    }
}