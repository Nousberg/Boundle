using UnityEngine;

namespace Assets.Scripts.Entities.Effects
{
    public class Godness : DamageEffect
    {
        public Godness(Entity target, int duration, float amplifier, bool infinite = false) : base(target, duration, amplifier, infinite)
        {
        }

        public override void ModifyDamage(ref float amount, DamageType type, Entity attacker)
        {
            if (type != DamageType.Generic)
                attacker.TakeDamage(amount * Mathf.Clamp01(Amplifier / 100f), attacker, DamageType.Generic);
        }
    }
}
