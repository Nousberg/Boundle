using System;
using System.Collections.Generic;

namespace Assets.Scripts.Saving.Data.InventoryData
{
    [Serializable]
    public class SavedInventory : SavedElement
    {
        public List<SavedItem> items = new List<SavedItem>();
        public int currentItem;

        public SavedInventory(List<SavedItem> items, int currentItem, int networkId) : base(networkId)
        {
            this.items = items;
            this.currentItem = currentItem;
        }
    }
}
