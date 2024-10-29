using System;

namespace Assets.Scripts.Entities.Effects
{
    public class Resistance : Effect, IDamageEffect<Resistance>
    {
        public Resistance(float duration, float amplifier, bool infinite) : base(duration, amplifier)
        {
        }

        public void ModifyDamage(ref float amount, Entity attacker, DamageType type)
        {
            amount /= Amplifier;
        }
    }
}
