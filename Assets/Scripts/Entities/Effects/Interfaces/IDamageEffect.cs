namespace Assets.Scripts.Entities.Effects
{
    public interface IDamageEffect<T> where T : Effect
    {
        void ModifyDamage(ref float amount, Entity attacker, DamageType type);
    }
}
