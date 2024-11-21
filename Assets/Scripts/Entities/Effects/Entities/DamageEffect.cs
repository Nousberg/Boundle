using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Entities.Effects
{
    public abstract class DamageEffect : Effect
    {
        protected readonly Entity target;

        public DamageEffect(Entity target, int duration, float amplifier, bool infinite = false) : base(duration, amplifier, infinite)
        {
            this.target = target;
            this.target.OnDamageTaken += ModifyDamage;
        }

        public abstract void ModifyDamage(ref float amount, DamageType type, Entity attacker);
    }
}
