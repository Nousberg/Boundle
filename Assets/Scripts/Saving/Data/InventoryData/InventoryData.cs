using System.Collections.Generic;

namespace Assets.Scripts.Saving
{
    public class InventoryData
    {
        public List<ItemDataContainer> items = new List<ItemDataContainer>();
        public int currentItem;

        public InventoryData(int currentItem, List<ItemDataContainer> items)
        {
            this.currentItem = currentItem;
            this.items = items;
        }
    }
}
