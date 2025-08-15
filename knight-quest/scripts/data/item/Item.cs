using Godot;

namespace Game.Data;

public partial class Item : Resource
{
    [Export] public string Name;
    [Export(PropertyHint.MultilineText)] public string Description;
    [Export] public Texture2D Icon;

    [Export] public int Cost;
    [Export] public bool Owned;
}