using Game.Components;
using Game.UI;
using Game.Utils;
using Godot;
using GodotUtilities;

namespace Game;

[Scene]
public partial class Battle : Node2D
{
    [Node] private HeadsUpDisplay headsUpDisplay;
    [Node] private RunManager runManager;
    [Node] private Node entities;

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }

    public override async void _Ready()
    {
        runManager.EncounterStarted += OnEncounterStarted;

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        var player = this.GetPlayer();

        player!.GlobalPosition = headsUpDisplay.PlayerGlobalPosition;
    }

    private void OnEncounterStarted(Entity[] enemies)
    {
        foreach (var enemy in enemies)
        {
            entities.AddChild(enemy);
            enemy.GlobalPosition = headsUpDisplay.EnemyGlobalPosition;
        }
    }
}