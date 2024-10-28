using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Inventory.Controllers
{
    public class UnmatchedInventoryController : InventoryManager
    {
        [Header("Properties")]
        [SerializeField] private float scanRadius;

        private void FixedUpdate()
        {
            Collider[] colliders = Physics.OverlapSphere(scanPosition.position, scanRadius);

            foreach (Collider collider in colliders)
            {
                DroppedItem item = collider.GetComponent<DroppedItem>();

                if (item != null)
                {
                    if(AddItem(item.item))
                        Destroy(item.gameObject);
                }
            }
        }
    }
}