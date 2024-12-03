namespace Assets.Scripts.Entities.Interfaces
{
    public interface IDamageable
    {
        void TakeDamage(float amount, Entity attacker, DamageType type);
    }
}
