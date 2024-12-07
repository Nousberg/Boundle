using Assets.Scripts.Inventory;

namespace Assets.Scripts.Effects
{
    public abstract class ItemEffect : Effect
    {
        protected readonly InventoryDataController inventory;

        public ItemEffect(InventoryDataController inventory, int duration, float amplifier, bool infinite = false) : base(duration, amplifier, infinite)
        {
            this.inventory = inventory;
        }
    }
}
