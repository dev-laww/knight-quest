using Godot;
using System;

[Tool, GlobalClass]
public partial class PartItem : Resource
{
    public enum PartType
    {
        Hair,
        Clothes,
        Head
    }

    [Export] public PartType Type { get; set; }    
    [Export] public Texture2D Icon { get; set; }
    [Export] public SpriteFrames AnimationFrames { get; set; }
    [Export(PropertyHint.MultilineText)] public string Description { get; set; } = "";
}