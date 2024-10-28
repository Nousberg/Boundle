namespace Assets.Scripts.Entities.Effects
{
    internal interface IDamageEffect<T> where T : Effect
    {
        void ModifyDamage(ref float amount, Entity attacker, DamageType type);
    }
}
