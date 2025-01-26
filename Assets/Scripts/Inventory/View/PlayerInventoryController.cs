using Assets.Scripts.Core.Input_System;
using Assets.Scripts.Core.InputSystem;
using Assets.Scripts.Inventory.DynamicData;
using Photon.Pun;
using UnityEngine;
using static UnityEngine.InputManagerEntry;

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

        public InputState inputSource;

        private InventoryDataController inventory => GetComponent<InventoryDataController>();
        private PhotonView view => GetComponent<PhotonView>();

        public void Init()
        {
            inputSource.InputRecieved += Equip;
            inputSource.InputRecieved += Drop;

            inventory.OnItemSwitched += ShowCurrentItem;
            inventory.OnItemAdded += ShowCurrentItem;
            inventory.OnItemRemoved += ShowCurrentItem;
        }
        private void Equip(InputHandler.InputBind bind)
        {
            if (bind != InputHandler.InputBind.EQUIP)
                return;

            RaycastHit hit;
            if (Physics.Raycast(holdPos.position, holdPos.forward, out hit, holdDist))
            {
                DroppedItem item = hit.collider.GetComponent<DroppedItem>();

                if (item != null && inventory.TryAddItem(item.Data))
                    Destroy(item.gameObject);
            }
        }
        private void Drop(InputHandler.InputBind bind)
        {
            if (bind != InputHandler.InputBind.DROP)
                return;

            DynamicItemData data = inventory.GetItems[inventory.CurrentItemIndex];

            if (inventory.TryRemoveItem(inventory.CurrentItemIndex))
            {
                GameObject instantiatedItem = Instantiate(data.data.prefab, holdPos.position, holdPos.rotation);
                instantiatedItem.AddComponent<DroppedItem>().Init(data);
                instantiatedItem.AddComponent<Rigidbody>();
                instantiatedItem.AddComponent<BoxCollider>().size = data.data.ColliderScale;
            }
        }
        private void Update()
        {
            if (view.IsMine)
                if (inputSource.VectorBinds[InputHandler.InputBind.MOUSEWHEEL].y != 0f)
                {
                    int targetIndex = inventory.CurrentItemIndex + (int)Input.mouseScrollDelta.y;

                    if (targetIndex < 0)
                        targetIndex = inventory.GetItems.Count - 1;
                    else if (targetIndex >= inventory.GetItems.Count)
                        targetIndex = 0;

                    inventory.SwitchItem(targetIndex);
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