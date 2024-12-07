using Assets.Scripts.Inventory.DynamicData;
using UnityEngine;

namespace Assets.Scripts.Inventory
{
    public class DroppedItem : MonoBehaviour
    {
        public DynamicItemData Data { get; private set; }

        public void Init(DynamicItemData data)
        {
            Data = data;
        }
    }
}