﻿using Assets.Scripts.Inventory.Tools;
using Assets.Scripts.Inventory.ItemTypes;
using Assets.Scripts.Inventory.Scriptables;
using UnityEngine;

namespace Assets.Scripts.Inventory.Controllers
{
    public class PlayerInventoryController : InventoryManager
    {
        [Header("Properties")]
        [SerializeField] private float raycastDistance;

        private void Update()
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                SetIndex(CurrentItem + (int)Input.mouseScrollDelta.y);
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                ItemContainer findedContainer = ItemReferences.Find(n => n.Data.Id == aviableItems[CurrentItem].data.Id);
                DroppedItem item = Instantiate(findedContainer.Data.Prefab, scanPosition.position, Quaternion.identity).AddComponent<DroppedItem>();
                item.Init(aviableItems[CurrentItem]);
                item.transform.localScale = findedContainer.Data.PrefabScale;
                RemoveItem(CurrentItemId, 1);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                RaycastHit hit;
                if (Physics.Raycast(scanPosition.position, scanPosition.forward, out hit, raycastDistance))
                {
                    DroppedItem item = hit.collider.GetComponent<DroppedItem>();

                    if (item != null)
                        if (AddItem(item.item))
                            Destroy(item.gameObject);
                }
            }
            if (aviableItems[CurrentItem] is WeaponItem weapon && aviableItems[CurrentItem].data is WeaponData weaponData)
            {
                switch (weaponData.WeaponType)
                {
                    case TypeOfWeapon.Bullet:
                        if (weapon.reloadTime > 0f)
                        {
                            weapon.reloadTime -= Time.deltaTime;

                            if (weapon.reloadTime <= 0f)
                            {
                                WeaponTool.Reload(weapon, weaponData);
                            }
                        }
                        if (Input.GetMouseButton(0) && weapon.fireTime <= Time.deltaTime && weapon.reloadTime <= 0f)
                        {
                            weapon.ammo--;
                            WeaponTool.Fire(weaponData, thisEntity, scanPosition);
                            InventoryChanged(weapon);

                            if (weapon.ammo <= 0)
                            {
                                weapon.ammo = 0;
                                weapon.reloadTime = weaponData.ReloadDuration;

                                ItemContainer findedContainer = ItemReferences.Find(n => n.Data.Id == weaponData.Id);
                                findedContainer.HandsAnimator.SetBool("Reload", true);
                                findedContainer.ItemAnimator.SetBool("Reload", true);

                                InventoryChanged(weapon);
                            }
                        }
                        if (Input.GetKeyDown(KeyCode.R) && weapon.ammo > 0 && weapon.ammo != weaponData.BaseAmmo)
                        {
                            weapon.ammo = 0;
                            weapon.reloadTime = weaponData.ReloadDuration;

                            ItemContainer findedContainer = ItemReferences.Find(n => n.Data.Id == weaponData.Id);
                            findedContainer.HandsAnimator.SetBool("Reload", true);
                            findedContainer.ItemAnimator.SetBool("Reload", true);

                            InventoryChanged(weapon);
                        }
                        break;
                    case TypeOfWeapon.Melee:
                        if (Input.GetMouseButton(0) && weapon.fireTime <= Time.deltaTime)
                        {
                            weapon.fireTime = Time.time + 1f / weaponData.FireRate;
                            WeaponTool.Fire(weaponData, thisEntity, scanPosition);
                        }
                        break;
                }
            }
        }
    }
}