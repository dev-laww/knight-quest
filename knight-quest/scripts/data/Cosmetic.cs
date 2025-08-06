using Godot;
using System;

namespace Game.Data;

[Tool, GlobalClass]
public partial class Cosmetic : Item
{
    public enum CosmeticType
    {
        Hair,
        Clothes,
        Head
    }

    public enum CosmeticCharacter
    {
        Knight,
        Archer,
        Mage,
        Gunslinger,
        Assassin
    }

    [Export] public CosmeticType Type;
    [Export] public CosmeticCharacter Character;
    [Export] public Texture2D Icon;
    [Export] public SpriteFrames AnimationFrames;
    [Export] public bool Equipped;
}