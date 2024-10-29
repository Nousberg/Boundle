using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Saving
{
    public class WeaponDataContainer : ItemDataContainer
    {
        public int currentAmmo, maxAmmo;
        public float currentOverheat;

        public WeaponDataContainer(int id, int quantity, float currentOverheat, int currentAmmo, int maxAmmo) : base(id, quantity)
        {
            this.currentOverheat = currentOverheat;
            this.currentAmmo = currentAmmo;
            this.maxAmmo = maxAmmo;
        }
    }
}
