using Godot;

namespace Game.Data;

public partial class Item : Resource
{
    [Export] public string Name;
    [Export] public string Description;

    [Export] public int Cost;
    [Export] public bool Owned;
}