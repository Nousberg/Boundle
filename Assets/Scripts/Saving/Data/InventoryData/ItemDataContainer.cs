using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Saving
{
    public class ItemDataContainer
    {
        public int id, quantity;

        public ItemDataContainer(int id, int quantity)
        {
            this.id = id;
            this.quantity = quantity;
        }
    }
}
