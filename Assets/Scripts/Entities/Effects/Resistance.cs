using System;

namespace Assets.Scripts.Entities.Effects
{
    public class Resistance : Effect, IDamageEffect<Effect>
    {
        public Resistance(float duration, float amplifier, bool infinite) : base(duration, amplifier, infinite)
        {
        }

        public void ModifyDamage(ref float amount, Entity attacker, DamageType type)
        {
            amount /= Amplifier;
        }
    }
}
