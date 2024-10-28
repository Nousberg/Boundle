using Assets.Scripts.Entities;
using Assets.Scripts.Inventory.ItemTypes;
using Assets.Scripts.Inventory.Scriptables;
using Assets.Scripts.Crafting;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets.Scripts.Inventory
{
    [RequireComponent(typeof(Entity))]
    public abstract class InventoryManager : MonoBehaviour
    {
        public const int MAX_ITEM_AMOUNT = 75;

        [field: Header("References")]
        [SerializeField] protected Transform scanPosition;
        [field: SerializeField] public List<ItemContainer> ItemReferences { get; private set; } = new List<ItemContainer>();
        [field: SerializeField] public List<CraftingMaterialData> Crafts = new List<CraftingMaterialData>();

        public event Action<DefaultItem, int> OnInventoryChanged;

        public int CurrentItemId { get; protected set; }
        public int CurrentItem { get; protected set; }
        protected List<DefaultItem> slots = new List<DefaultItem>();
        protected Entity thisEntity => GetComponent<Entity>();

        private void Start()
        {
            slots.Add(new DefaultItem(ItemReferences.Find(n => n.Data.Id == 0).Data, 1));
        }

        public void SetIndex(int index)
        {
            if (index < 0 || index >= ItemReferences.Count)
                return;

            CurrentItem = index;
            CurrentItemId = slots[CurrentItem].data.Id;
            OnInventoryChanged?.Invoke(slots[CurrentItem], CurrentItem);
        }
        public bool AddItem(DefaultItem item)
        {
            if (slots.Count < MAX_ITEM_AMOUNT)
            {
                DefaultItem slot = slots.Find(n => n.data.Id == item.data.Id);

                if (slot != null)
                {
                    if (item.quantity > 0)
                    {
                        slot.quantity += Math.Min(item.quantity + slot.quantity, slot.data.MaxQuantity);
                    }
                }
                else if (item.quantity > 0)
                {
                    ItemContainer findedContainer = ItemReferences.Find(n => n.Data.Id == item.data.Id);
                    slots.Add(item);
                }

                if (item.quantity > 0 && slot != null)
                {
                    CurrentItem = slots.IndexOf(slot);
                    CurrentItemId = slots[CurrentItem].data.Id;
                    OnInventoryChanged?.Invoke(new DefaultItem(slot.data, slot.quantity), CurrentItem);
                    UpdateCurrentItem();

                    return true;
                }
            }

            return false;
        }
        public void RemoveItem(int id, int quantity)
        {
            if (quantity > 0)
            {
                DefaultItem slot = slots.Find(n => n.data.Id == id);

                if (slot != null)
                    slot.quantity = Math.Min(slot.quantity - quantity, 0);
            }
        }
        public void CraftItem(int id, int quantity)
        {
            List<DefaultItem> slotsToRemove = new List<DefaultItem>();
            int requiredSum = Crafts.Find(n => n.Id == id).RequiredItems.Count;
            int resultedSum = 0;

            for (int i = 0; i < quantity; i++)
            {
                resultedSum = 0;

                foreach (var craft in Crafts)
                {
                    if (id == craft.Id)
                    {
                        foreach (var recipe in craft.RequiredItems)
                        {
                            foreach (var item in slots)
                            {
                                if (item.data.Id == recipe.Id && item.quantity == recipe.Quantity)
                                {
                                    requiredSum++;
                                    slotsToRemove.Add(item);
                                }
                            }
                        }
                        break;
                    }
                }

                if (resultedSum == requiredSum)
                {
                    foreach (var slot in slotsToRemove)
                        RemoveItem(slot.data.Id, slot.quantity);

                    ItemData item = ItemReferences.Find(n => n.Data.Id == id).Data;

                    if (item is ItemData data)
                    {
                        AddItem(new DefaultItem(data));
                    }
                    else if (item is WeaponData weaponData)
                    {
                        AddItem(new WeaponItem(weaponData, 0f, weaponData.BaseAmmo, weaponData.BaseAmmo));
                    }
                }
            }
        }
        private void UpdateCurrentItem()
        {
            ItemContainer findedContainer = ItemReferences.Find(n => n.Data.Id == slots[CurrentItem].data.Id);

            if (findedContainer != null)
            {
                foreach (var itemRef in ItemReferences)
                {
                    itemRef.gameObject.SetActive(false);
                }

                findedContainer.gameObject.SetActive(true);
            }
        }

        protected void InventoryChanged(DefaultItem item)
        {
            OnInventoryChanged?.Invoke(item, CurrentItem);
        }
        protected void Reload(WeaponItem weaponContainer, WeaponData weaponData)
        {
            int v = weaponContainer.maxAmmo;
            weaponContainer.maxAmmo -= Mathf.Clamp(weaponData.BaseAmmo - weaponContainer.ammo, 0, weaponContainer.maxAmmo);
            weaponContainer.ammo += Mathf.Clamp(weaponData.BaseAmmo - weaponContainer.ammo, 0, v);
        }
        protected void Fire(WeaponData data)
        {
            RaycastHit hit;

            for (int i = 0; i < data.BulletsPerShoot; i++)
                if (Physics.Raycast(scanPosition.position, scanPosition.forward, out hit, data.Range))
                {
                    Entity entity = hit.collider.GetComponent<Entity>();

                    if (entity != null)
                    {
                        entity.TakeDamage(data.Damage, thisEntity, data.TypeOfDamage);
                    }
                }
        }
    }
}