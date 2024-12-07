using Assets.Scripts.Inventory.DynamicData;
using UnityEngine;

namespace Assets.Scripts.Inventory.View
{
    [RequireComponent(typeof(InventoryDataController))]
    public class PlayerInventoryController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform holdPos;

        [Header("Properties")]
        [SerializeField] private KeyCode equipBind;
        [SerializeField] private KeyCode dropBind;
        [SerializeField] private float holdDist;

        private InventoryDataController inventory => GetComponent<InventoryDataController>();

        private void Start()
        {
            inventory.OnItemSwitched += ShowCurrentItem;
            inventory.OnItemAdded += ShowCurrentItem;
            inventory.OnItemRemoved += ShowCurrentItem;
        }
        private void Update()
        {
            if (Input.mouseScrollDelta.y != 0f)
            {
                int targetIndex = inventory.CurrentItemIndex - (int)Input.mouseScrollDelta.y;

                if (targetIndex < 0)
                    targetIndex = inventory.GetItems.Count - 1;
                else if (targetIndex >= inventory.GetItems.Count)
                    targetIndex = 0;

                inventory.SwitchItem(targetIndex);
            }
            if (Input.GetKeyDown(equipBind))
            {
                RaycastHit hit;
                if (Physics.Raycast(holdPos.position, holdPos.forward, out hit, holdDist))
                {
                    DroppedItem item = hit.collider.GetComponent<DroppedItem>();

                    if (item != null)
                        inventory.TryAddItem(item.Data);
                }
            }
            if (Input.GetKeyDown(dropBind))
            {
                DynamicItemData data = inventory.GetItems[inventory.CurrentItemIndex];

                if (inventory.TryRemoveItem(inventory.CurrentItemIndex))
                    Instantiate(data.data.prefab, holdPos.position, holdPos.rotation).AddComponent<DroppedItem>().Init(data);
            }
        }
        private void ShowCurrentItem()
        {
            foreach (var item in inventory.AllInGameItems)
                item.gameObject.SetActive(false);

            inventory.AllInGameItems
                .Find(n => n.BaseData.Id == inventory.GetItems[inventory.CurrentItemIndex].data.Id)
                .gameObject.SetActive(true);
        }
    }
}