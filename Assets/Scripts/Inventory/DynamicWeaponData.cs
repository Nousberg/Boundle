using Assets.Scripts.Entities.Effects.Inventory;
using Assets.Scripts.Inventory.Scriptables;
using System.Collections.Generic;

namespace Assets.Scripts.Inventory
{
    public class DynamicWeaponData : DynamicItemData
    {
        public int currentAmmo;
        public int overallAmmo;
        public float reloadTime;
        public float fireTime;
        public float overheat;

        public DynamicWeaponData(BaseItemData data, int currentAmmo, int overallAmmo, float overheat, List<ItemEffect> effects = null) : base(data, effects)
        {
            this.currentAmmo = currentAmmo;
            this.overallAmmo = overallAmmo;
            this.overheat = overheat;
        }
    }
}
