using Assets.Scripts.Effects;
using Assets.Scripts.Inventory.Scriptables;
using System.Collections.Generic;

namespace Assets.Scripts.Inventory.DynamicData
{
    public class DynamicItemData
    {
        public readonly BaseItemData data;

        public DynamicItemData(BaseItemData data)
        {
            this.data = data;
        }
    }
}
