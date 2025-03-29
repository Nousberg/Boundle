using Assets.Scripts.Inventory.Scriptables;

namespace Assets.Scripts.Inventory.DynamicData
{
    public class DynamicWeaponData : DynamicItemData
    {
        public int currentAmmo;
        public int overallAmmo;
        public float reloadTime;
        public float fireTime;
        public float overheat;

        public DynamicWeaponData(BaseItemData data, int currentAmmo, int overallAmmo, float overheat) : base(data)
        {
            this.currentAmmo = currentAmmo;
            this.overallAmmo = overallAmmo;
            this.overheat = overheat;
        }
    }
}
