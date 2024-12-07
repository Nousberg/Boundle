using Assets.Scripts.Inventory;
using UnityEngine;

namespace Assets.Scripts.Effects
{
    public abstract class WeaponDamageEffect : ItemEffect
    {
        public WeaponDamageEffect(InventoryDataController inventory, int duration, float amplifier, bool infinite = false) : base(inventory, duration, amplifier, infinite)
        {
            foreach (var item in inventory.GetItems)
            {
                ItemDataController controller = inventory.AllInGameItems.Find(n => n.BaseData.Id == item.data.Id);

                if (controller != null && controller is WeaponDataController weapon)
                    weapon.OnFire += ModifyDamage;
            }
        }

        public abstract void ModifyDamage(ref float damage);
    }
}
