using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Saving
{
    public class GameDataContainer
    {
        public List<EntityData> entities = new List<EntityData>();

        public GameDataContainer(List<EntityData> entities)
        {
            this.entities = entities;
        }
    }
}
