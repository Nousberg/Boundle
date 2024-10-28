using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Saving
{
    public class ItemDataContainer
    {
        public int id;
        public int currentAmmo, maxAmmo;
        public float currentOverheat;

        public ItemDataContainer(int id, float currentOverheat, int currentAmmo, int maxAmmo)
        {
            this.id = id;
            this.currentOverheat = currentOverheat;
            this.currentAmmo = currentAmmo;
            this.maxAmmo = maxAmmo;
        }
    }
}
