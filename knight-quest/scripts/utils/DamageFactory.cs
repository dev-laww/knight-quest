using System.Collections.Generic;
using Game.Data;
using GodotUtilities;

namespace Game.Utils;

public class DamageFactory
{
    public class AttackBuilder(Entity source)
    {
        // TODO: move crit chance to stats manager
        private const float CRITICAL_CHANCE = 0.2f;

        private readonly List<StatusEffect.Info> statusEffectPool = [];

        public AttackBuilder SetStatusEffectPool(IEnumerable<StatusEffect.Info> pool)
        {
            statusEffectPool.AddRange(pool);
            return this;
        }

        public Attack Build()
        {
            var effects = new List<StatusEffect>();

            // foreach (var effect in statusEffectPool)
            // {
            //     var randomNumber = MathUtil.RNG.RandfRange(0, 1);
            //
            //     if (randomNumber > effect.Chance && !effect.IsGuaranteed) continue;
            //
            //     var instance = StatusEffectRegistry.Get(effect.Id);
            //
            //     if (instance == null) continue;
            //
            //     effects.Add(instance);
            // }

            var isCritical = MathUtil.RNG.RandfRange(0, 1) < CRITICAL_CHANCE;
            var damage = source.StatsManager.Damage;
            var calculatedDamage = damage * (isCritical ? 2 : 1);

            var attack = new Attack
            {
                Damage = calculatedDamage,
                Source = source,
                StatusEffects = effects,
            };

            return attack;
        }
    }
}