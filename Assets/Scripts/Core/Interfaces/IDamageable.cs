using Assets.Scripts.Entities;

namespace Assets.Scripts.Core.Interfaces
{
    public interface IDamageable
    {
        void TakeDamage(float amount, Entity attacker, DamageData.DamageType type);
    }
}
