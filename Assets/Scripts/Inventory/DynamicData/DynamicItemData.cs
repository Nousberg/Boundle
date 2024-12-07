using Assets.Scripts.Effects;
using Assets.Scripts.Inventory.Scriptables;
using System.Collections.Generic;

namespace Assets.Scripts.Inventory.DynamicData
{
    public class DynamicItemData
    {
        public readonly BaseItemData data;
        public List<Effect> effects = new List<Effect>();

        public DynamicItemData(BaseItemData data, List<Effect> effects = null)
        {
            this.data = data;

            if (effects != null)
                this.effects = effects;
        }
    }
}
