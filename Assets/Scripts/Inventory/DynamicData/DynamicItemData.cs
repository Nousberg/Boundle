using Assets.Scripts.Inventory.Scriptables;

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
