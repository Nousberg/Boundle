using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Inventory.ItemTypes
{
    public class WeaponItem : DefaultItem
    {
        public float overheat, fireTime, reloadTime;
        public int ammo, maxAmmo;

        public WeaponItem(ItemData data, float overheat, int ammo, int maxAmmo, int quantity = 1) : base(data, quantity)
        {
            this.overheat = overheat;
            this.maxAmmo = maxAmmo;
            this.ammo = ammo;
        }
    }
}
