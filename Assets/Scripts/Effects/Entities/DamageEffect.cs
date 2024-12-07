using Assets.Scripts.Entities;

namespace Assets.Scripts.Effects
{
    public abstract class DamageEffect : Effect
    {
        protected readonly Entity target;

        public DamageEffect(Entity target, int duration, float amplifier, bool infinite = false) : base(duration, amplifier, infinite)
        {
            this.target = target;
            this.target.OnDamageTakenForEffects += ModifyDamage;
        }

        public abstract void ModifyDamage(ref float amount, DamageType type, Entity attacker);
    }
}
