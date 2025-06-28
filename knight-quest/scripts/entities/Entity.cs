using Game.Components;
using Godot;
using GodotUtilities;
using GodotUtilities.Util;

namespace Game;

[Scene]
public partial class Entity : Node2D
{
    [Node] public StatsManager StatsManager;

    [Signal] public delegate void DiedEventHandler();

    public bool IsAlive => StatsManager.Health > 0;

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }

    public override void _Ready()
    {
        StatsManager.StatDepleted += OnStatDepleted;
    }

    public virtual void TakeTurn(Entity target)
    {
        var targetStatsManager = target.StatsManager;

        targetStatsManager.TakeDamage(StatsManager.Damage);
        Logger.Debug($"{Name} attacks {target.Name} for {StatsManager.Damage} damage!");

        // TODO: Play animations, etc.
    }

    private void OnStatDepleted(StatsManager.Stat stat)
    {
        if (stat != StatsManager.Stat.Health) return;

        Logger.Debug($"{Name} has been defeated!");
        QueueFree();
        EmitSignalDied();
    }
}