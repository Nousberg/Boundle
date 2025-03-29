using UnityEngine;

namespace Assets.Scripts.Inventory
{
    public class DroppedItem : MonoBehaviour
    {
        [HideInInspector] public string data;

        public void Init(string data) => this.data = data;
    }
}