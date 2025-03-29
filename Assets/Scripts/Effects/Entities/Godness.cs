using UnityEngine;
using Assets.Scripts.Entities;

namespace Assets.Scripts.Effects
{
    public class Godness : DamageEffect
    {
        public Godness(Entity target, int duration, float amplifier, bool infinite = false) : base(target, duration, amplifier, infinite)
        {
        }

        public override void ModifyDamage(ref float amount, DamageData.DamageType type, Entity attacker)
        {
            attacker.TakeDamage(amount * Mathf.Clamp01(Amplifier / 100f), attacker, DamageData.DamageType.Generic);
        }
        public override void CombineEffects(Effect effect)
        {
            Amplifier += effect.Amplifier;
            SetLifetime(effect.Duration + RemainingLifetime);
        }
    }
}
