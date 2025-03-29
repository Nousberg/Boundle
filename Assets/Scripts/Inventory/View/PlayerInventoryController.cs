using Assets.Scripts.Core.Input_System;
using Assets.Scripts.Core.InputSystem;
using Assets.Scripts.Inventory.DynamicData;
using Assets.Scripts.Saving.Data.InventoryData;
using Newtonsoft.Json;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Inventory.View
{
    [RequireComponent(typeof(InventoryDataController))]
    public class PlayerInventoryController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform holdPos;

        [Header("Properties")]
        [SerializeField] private float holdDist;

        private InventoryDataController inventory => GetComponent<InventoryDataController>();
        private PhotonView view => GetComponent<PhotonView>();

        private InputState inputSource;

        public void Init(InputState inputSource)
        {
            this.inputSource = inputSource;

            inputSource.InputRecieved += Equip;
            inputSource.InputRecieved += Drop;

            inventory.OnItemSwitched += () => ShowCurrentItem(string.Empty);
            inventory.OnItemAdded += ShowCurrentItem;
            inventory.OnItemRemoved += ShowCurrentItem;
            inventory.OnClear += NativeDrop;
        }
        private void Equip(InputHandler.InputBind bind)
        {
            if (bind != InputHandler.InputBind.EQUIP)
                return;

            RaycastHit hit;
            if (Physics.Raycast(holdPos.position, holdPos.forward, out hit, holdDist))
            {
                DroppedItem item = hit.collider.GetComponent<DroppedItem>();

                if (item != null)
                {
                    SavedItem sItem = JsonConvert.DeserializeObject<SavedItem>(item.data);
                    ItemDataController cData = inventory.AllInGameItems.Find(n => n.BaseData.Id == sItem.id);

                    if (cData != null)
                    {
                        if (cData is WeaponDataController)
                        {
                            DynamicWeaponData weaponInstance = new DynamicWeaponData(
                                cData.BaseData,
                                sItem.intAttributes["ammo"],
                                sItem.intAttributes["overallAmmo"],
                                sItem.floatAttributes["overheat"]);

                            inventory.TryAddItem(weaponInstance);
                        }
                        else if (cData is ItemDataController)
                            inventory.TryAddItem(new DynamicItemData(cData.BaseData));

                        hit.collider.GetComponent<PhotonView>().RequestOwnership();
                        PhotonNetwork.Destroy(hit.collider.gameObject);
                    }
                }
            }
        }
        private void Drop(InputHandler.InputBind bind)
        {
            if (bind != InputHandler.InputBind.DROP)
                return;

            DynamicItemData data = inventory.GetItems[inventory.CurrentItemIndex];

            if (inventory.TryRemoveItem(inventory.CurrentItemIndex))
            {
                string networkItemData = string.Empty;

                if (data is DynamicWeaponData weaponData)
                {
                    SavedItem savedWeapon = new SavedItem(data.data.Id, new Dictionary<string, float>(), new Dictionary<string, int>(), -1);
                    savedWeapon.intAttributes.Add("ammo", weaponData.currentAmmo);
                    savedWeapon.intAttributes.Add("overallAmmo", weaponData.overallAmmo);
                    savedWeapon.floatAttributes.Add("overheat", weaponData.overheat);

                    networkItemData = JsonConvert.SerializeObject(savedWeapon);
                }
                else
                    networkItemData = JsonConvert.SerializeObject(new SavedItem(data.data.Id, new Dictionary<string, float>(), new Dictionary<string, int>(), -1));

                GameObject instantiatedItem = PhotonNetwork.InstantiateRoomObject(data.data.prefab, holdPos.position, holdPos.rotation);
                instantiatedItem.GetComponent<DroppedItem>().Init(networkItemData);
            }
        }
        private void NativeDrop(int index)
        {
            DynamicItemData data = inventory.GetItems[index];
            string networkItemData = string.Empty;

            if (data is DynamicWeaponData weaponData)
            {
                SavedItem savedWeapon = new SavedItem(data.data.Id, new Dictionary<string, float>(), new Dictionary<string, int>(), -1);
                savedWeapon.intAttributes.Add("ammo", weaponData.currentAmmo);
                savedWeapon.intAttributes.Add("overallAmmo", weaponData.overallAmmo);
                savedWeapon.floatAttributes.Add("overheat", weaponData.overheat);

                networkItemData = JsonConvert.SerializeObject(savedWeapon);
            }
            else
                networkItemData = JsonConvert.SerializeObject(new SavedItem(data.data.Id, new Dictionary<string, float>(), new Dictionary<string, int>(), -1));

            GameObject instantiatedItem = PhotonNetwork.Instantiate(data.data.prefab, holdPos.position, holdPos.rotation);
            instantiatedItem.GetComponent<DroppedItem>().Init(networkItemData);
        }
        private void Update()
        {
            if (!view.IsMine)
                return;

            if (inputSource.VectorBinds[InputHandler.InputBind.MOUSEWHEEL].y != 0f)
            {
                int targetIndex = inventory.CurrentItemIndex + (int)inputSource.VectorBinds[InputHandler.InputBind.MOUSEWHEEL].y;
                if (targetIndex < 0)
                    targetIndex = inventory.GetItems.Count - 1;
                else if (targetIndex >= inventory.GetItems.Count)
                    targetIndex = 0;
                inventory.SwitchItem(targetIndex);
            }
        }
        private void ShowCurrentItem(string name)
        {
            foreach (var item in inventory.AllInGameItems)
                item.gameObject.SetActive(false);

            inventory.AllInGameItems
                .Find(n => n.BaseData.Id == inventory.GetItems[inventory.CurrentItemIndex].data.Id)
                .gameObject.SetActive(true);
        }
    }
}