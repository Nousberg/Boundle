using Assets.Scripts.Entities;

namespace Assets.Scripts.Core.Interfaces
{
    public interface IDamageable
    {
        void TakeDamage(float amount, Entity attacker, DamageType type);
    }
}
