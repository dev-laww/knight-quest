using Godot;

namespace Game.Data;

[GlobalClass]
public partial class RunConfiguration : Resource
{
    [Export] public Encounter[] Encounters = [];
    [Export] public int TurnDuration = 30;
}