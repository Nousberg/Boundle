using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Saving
{
    public class InventoryData
    {
        public int currentItem;

        public InventoryData(int currentItem)
        {
            this.currentItem = currentItem;
        }
    }
}
