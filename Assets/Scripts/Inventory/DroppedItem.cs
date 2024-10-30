using Assets.Scripts.Entities;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Inventory
{
    public class DroppedItem : MonoBehaviour
    {
        public DefaultItem item { get; private set; }

        public void Init(DefaultItem item)
        {
            this.item = item;
        }
    }
}