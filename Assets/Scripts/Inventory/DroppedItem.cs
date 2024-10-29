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

        private void OnCollisionStay(Collision collision)
        {
            Entity e = collision.gameObject.GetComponent<Entity>();
            if (e != null)
                e.TakeDamage(10f, e, DamageType.Generic);
        }
    }
}