using Godot;

namespace Game.Data;

[GlobalClass]
public abstract partial class Consumable : Item
{
    [Export] public int Quantity;
    [Export] public StatusEffect[] StatusEffects = [];

    public abstract void Use(Entity target);
}