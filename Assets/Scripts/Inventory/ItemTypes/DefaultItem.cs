namespace Assets.Scripts.Inventory
{
    public class DefaultItem
    {
        public ItemData data;
        public int quantity;

        public DefaultItem(ItemData data, int quantity = 1)
        {
            this.data = data;
            this.quantity = quantity;
        }
    }
}
