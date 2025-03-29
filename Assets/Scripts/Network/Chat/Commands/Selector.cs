using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Network.Chat.Commands
{
    public class Selector
    {
        public List<EntityNetworkData> targets = new List<EntityNetworkData>();

        public Selector(List<EntityNetworkData> targets)
        {
            this.targets = targets;
        }
    }
}
