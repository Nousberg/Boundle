using System;

namespace Assets.Scripts.Entities.Effects
{
    public class Resistance : DamageEffect
    {
        public Resistance(Entity target, int duration, float amplifier, bool infinite = false) : base(target, duration, amplifier, infinite)
        {
        }

        public override void ModifyDamage(ref float amount, DamageType type, Entity attacker)
        {
            amount *= 100f / (100f + Amplifier);
        }
    }
}
