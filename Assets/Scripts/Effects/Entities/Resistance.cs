using Assets.Scripts.Entities;

namespace Assets.Scripts.Effects
{
    public class Resistance : DamageEffect
    {
        public Resistance(Entity target, int duration, float amplifier, bool infinite = false) : base(target, duration, amplifier, infinite)
        {
        }

        public override void ModifyDamage(ref float amount, DamageData.DamageType type, Entity attacker)
        {
            amount *= 100f / (100f + Amplifier);
        }
        public override void CombineEffects(Effect effect)
        {
            Amplifier += effect.Amplifier;
        }
    }
}
