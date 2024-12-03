using Assets.Scripts.Inventory;
using UnityEngine;

namespace Assets.Scripts.Entities.Effects.Inventory
{
    public class Supression : WeaponDamageEffect
    {
        public Supression(InventoryDataController inventory, int duration, float amplifier, bool infinite = false) : base(inventory, duration, amplifier, infinite)
        {

        }

        public override void ModifyDamage(ref float damage)
        {
            damage *= 1f - Mathf.Clamp01(Amplifier / 100f);
        }
        public override void CombineEffects(Effect effect)
        {
            Amplifier += effect.Amplifier;
        }
    }
}
