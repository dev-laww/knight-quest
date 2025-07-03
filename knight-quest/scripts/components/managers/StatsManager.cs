using System;
using System.Collections.Generic;
using Game.Data;
using Godot;
using Game.Utils;

namespace Game.Components;

[GlobalClass]
public partial class StatsManager : Node
{
    [Signal] public delegate void StatChangedEventHandler(int value, Stat stat);
    [Signal] public delegate void StatIncreasedEventHandler(int value, Stat stat);
    [Signal] public delegate void StatDecreasedEventHandler(int value, Stat stat);
    [Signal] public delegate void StatDepletedEventHandler(Stat stat);
    [Signal] public delegate void DamageTakenEventHandler(int amount);
    [Signal] public delegate void AttackReceivedEventHandler(Attack attack);
    [Signal] public delegate void StatusEffectAddedEventHandler(StatusEffect effect);
    [Signal] public delegate void StatusEffectRemovedEventHandler(StatusEffect effect);

    [Export] private bool Invulnerable;

    [Export] public int MaxHealth = 10;

    public int Health
    {
        get => health;
        private set => SetStat(ref health, value, Stat.Health);
    }

    [Export]
    public int Damage
    {
        get => damage;
        private set => SetStat(ref damage, value, Stat.Damage);
    }

    private int damage = 1;
    private int health;
    private readonly Dictionary<string, int> speedModifiers = [];

    private int baseMaxHealth;
    private int baseDamage;
    private Dictionary<string, StatusEffect> statusEffects = [];
    private Entity entity;

    public override void _Ready()
    {
        health = MaxHealth;

        baseMaxHealth = MaxHealth;
        baseDamage = Damage;
        entity = GetParentOrNull<Entity>();

        if (entity != null) return;

        Logger.Error("StatsManager must be a child of an Entity node.");
    }

    public void Heal(int amount, ModifyMode mode = ModifyMode.Value)
    {
        Health += mode switch
        {
            ModifyMode.Percentage => Mathf.RoundToInt(MaxHealth * (amount / 100f)),
            ModifyMode.Value => amount,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }

    public void TakeDamage(int amount, ModifyMode mode = ModifyMode.Value)
    {
        if (Invulnerable) return;

        Health -= mode switch
        {
            ModifyMode.Percentage => Mathf.RoundToInt(MaxHealth * (amount / 100f)),
            ModifyMode.Value => amount,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

        EmitSignalDamageTaken(amount);
    }

    // Damage
    public void IncreaseDamage(int amount, ModifyMode mode = ModifyMode.Value)
    {
        Damage += mode switch
        {
            ModifyMode.Percentage => Mathf.RoundToInt(Damage * (amount / 100f)),
            ModifyMode.Value => amount,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }

    public void DecreaseDamage(int amount, ModifyMode mode = ModifyMode.Value)
    {
        Damage -= mode switch
        {
            ModifyMode.Percentage => Mathf.RoundToInt(Damage * (amount / 100f)),
            ModifyMode.Value => amount,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }

    public void SetInvulnerable(bool value)
    {
        Invulnerable = value;
    }

    private void SetStat(ref int stat, int value, Stat statType)
    {
        if (stat == value) return;

        var oldValue = stat;
        stat = Math.Clamp(value, 0, statType switch
        {
            Stat.Health => MaxHealth,
            _ => int.MaxValue
        });

        var diff = Math.Abs(value - oldValue);

        EmitSignalStatChanged(value, statType);
        EmitSignal(value > oldValue ? SignalName.StatIncreased : SignalName.StatDecreased, diff, (int)statType);

        if (value <= 0)
            EmitSignalStatDepleted(statType);
    }

    public void ReceiveAttack(Attack attack)
    {
        TakeDamage(attack.Damage);

        if (!attack.HasStatusEffects) return;

        foreach (var effect in attack.StatusEffects)
            AddStatusEffect(effect);
    }

    public void AddStatusEffect(StatusEffect effect)
    {
        if (effect == null) return;

        effect.ApplyStatusEffect(entity);
        statusEffects.Add(effect.Id, effect);
        EmitSignalStatusEffectAdded(effect);

        Logger.Debug($"Status effect added {effect.Name} to {entity.Name}.");
    }

    public void RemoveStatusEffect(string id)
    {
        if (!statusEffects.TryGetValue(id, out var effect)) return;

        effect.Remove();
        statusEffects.Remove(id);
        EmitSignalStatusEffectRemoved(effect);

        Logger.Debug($"Status effect removed {effect.Name} from {entity.Name}.");
    }

    public void TickStatusEffects()
    {
        foreach (var effect in statusEffects.Values)
        {
            effect.Update();
            if (effect.RemainingDuration > 0) continue;

            RemoveStatusEffect(effect.Id);
        }
    }

    public enum ModifyMode
    {
        Percentage,
        Value
    }

    public enum Stat
    {
        Damage,
        Health,
    }
}