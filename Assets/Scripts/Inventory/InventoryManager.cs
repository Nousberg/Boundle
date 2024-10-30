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
        [field: SerializeField] public List<CraftingRecipeData> CraftRecipes = new List<CraftingRecipeData>();

        public event Action<DefaultItem, int> OnInventoryChanged;

        public int CurrentItemId { get; protected set; }
        public int CurrentItem { get; protected set; }
        protected List<DefaultItem> aviableItems = new List<DefaultItem>();
        protected Entity thisEntity => GetComponent<Entity>();

        private void Start()
        {
            aviableItems.Add(new DefaultItem(ItemReferences.Find(n => n.Data.Id == 0).Data, 1));
        }

        public void SetIndex(int index)
        {
            if (index < 0 || index >= ItemReferences.Count)
                return;

            CurrentItem = index;
            CurrentItemId = aviableItems[CurrentItem].data.Id;
            OnInventoryChanged?.Invoke(aviableItems[CurrentItem], CurrentItem);
        }
        public bool AddItem(DefaultItem item)
        {
            if (aviableItems.Count < MAX_ITEM_AMOUNT)
            {
                DefaultItem slot = aviableItems.Find(n => n.data.Id == item.data.Id);

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
                    aviableItems.Add(item);
                }

                if (item.quantity > 0 && slot != null)
                {
                    CurrentItem = aviableItems.IndexOf(slot);
                    CurrentItemId = aviableItems[CurrentItem].data.Id;
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
                DefaultItem slot = aviableItems.Find(n => n.data.Id == id);

                if (slot != null)
                    slot.quantity = Math.Min(slot.quantity - quantity, 0);
            }
        }
        public void CraftItem(int id, int quantity)
        {
            int requiredSum = CraftRecipes.Find(n => n.Id == id).RequiredItems.Count;
            int resultedSum = 0;

            foreach (var recipe in CraftRecipes)
            {
                if (id == recipe.Id)
                {
                    foreach (var craftMaterial in recipe.RequiredItems)
                    {
                        List<DefaultItem> items = aviableItems.FindAll(n => n.data.Id == craftMaterial.Id);
                        int sum = items.Sum(n => n.quantity);

                        foreach (var item in items)
                            RemoveItem(item.data.Id, craftMaterial.Quantity / items.Count);

                        if (sum >= craftMaterial.Quantity)
                        {
                            resultedSum++;
                        }
                    }
                    break;
                }
            }

            if (resultedSum == requiredSum)
            {
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
        public List<DefaultItem> GetAllItems()
        {
            return aviableItems;
        }
        private void UpdateCurrentItem()
        {
            ItemContainer findedContainer = ItemReferences.Find(n => n.Data.Id == aviableItems[CurrentItem].data.Id);

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
            UpdateCurrentItem();
            OnInventoryChanged?.Invoke(item, CurrentItem);
        }
    }
}