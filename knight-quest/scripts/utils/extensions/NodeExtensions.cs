using System.Linq;
using Game.Components;
using Game.Entities;
using Godot;

namespace Game.Utils;

#nullable enable
public static class NodeExtensions
{
    public static Player? GetPlayer(this Node node)
    {
        var player = node.GetTree().GetNodesInGroup("Player").FirstOrDefault();

        return player as Player;
    }

    public static RunManager? GetRunManager(this Node node)
    {
        var runManager = node.GetTree().GetNodesInGroup("RunManager").FirstOrDefault();

        return runManager as RunManager;
    }
}