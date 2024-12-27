using Assets.Scripts.Entities;
using Assets.Scripts.Network;

namespace Assets.Scripts.Core.Interfaces
{
    public interface IDamageable
    {
        void TakeDamage(float amount, LocalEntity attacker, DamageType type);
    }
}
