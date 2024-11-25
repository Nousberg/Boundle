using Assets.Scripts.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Entities.Effects.Inventory
{
    public abstract class WeaponDamageEffect : ItemEffect
    {
        public WeaponDamageEffect(InventoryDataController inventory, int duration, float amplifier, bool infinite = false) : base(inventory, duration, amplifier, infinite)
        {
            foreach (var item in inventory.GetItems)
            {
                ItemDataController controller = inventory.AllInGameItems.Find(n => n.Data.Id == item.data.Id);

                if (controller != null && controller is WeaponDataController weapon)
                    weapon.OnFire += ModifyDamage;
            }
        }

        public abstract void ModifyDamage(ref float damage);
    }
}
