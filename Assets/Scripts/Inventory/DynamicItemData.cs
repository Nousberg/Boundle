using Assets.Scripts.Entities.Effects;
using Assets.Scripts.Entities.Effects.Inventory;
using Assets.Scripts.Inventory.Scriptables;
using System.Collections.Generic;

namespace Assets.Scripts.Inventory
{
    public class DynamicItemData
    {
        public readonly BaseItemData data;
        public List<ItemEffect> effects = new List<ItemEffect>();

        public DynamicItemData(BaseItemData data, List<ItemEffect> effects = null)
        {
            this.data = data;

            if (effects != null)
                this.effects = effects;
        }
    }
}
