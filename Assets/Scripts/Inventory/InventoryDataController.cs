using Assets.Scripts.Effects;
using Assets.Scripts.Inventory.DynamicData;
using Assets.Scripts.Inventory.Scriptables;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Inventory
{
    public class InventoryDataController : MonoBehaviour
    {
        [field: SerializeField] public List<ItemDataController> AllInGameItems { get; private set; } = new List<ItemDataController>();
        [field: SerializeField] public List<ItemDataController> DefaultItems { get; private set; } = new List<ItemDataController>();
        [field: Min(0f)][field: SerializeField] public float MaxInventoryWeight { get; private set; }

        public event Action OnItemSwitched;
        public event Action OnItemAdded;
        public event Action OnItemRemoved;

        public int CurrentItemIndex { get; private set; }

        public float InventoryWeight => aviableItems.Sum(n => n.data.Weight);
        public List<DynamicItemData> GetItems => aviableItems;

        private List<DynamicItemData> aviableItems = new List<DynamicItemData>();

        public void Init()
        {
            foreach (var item in DefaultItems)
            {
                if (!(item.BaseData is BaseWeaponData))
                    aviableItems.Add(new DynamicItemData(item.BaseData));
                else if (item.BaseData is BaseWeaponData data)
                    aviableItems.Add(new DynamicWeaponData(data, data.BaseAmmo, data.BaseAmmo, 0f));
            }

            OnItemAdded?.Invoke();
        }

        public void SwitchItem(int index)
        {
            if (index < 0 || index >= aviableItems.Count)
                return;

            AllInGameItems.Find(n => n.BaseData.Id == aviableItems[index].data.Id).InjectData(aviableItems[index]);

            CurrentItemIndex = index;
            OnItemSwitched?.Invoke();
        }
        public bool TryAddItem(DynamicItemData item)
        {
            if (InventoryWeight + item.data.Weight <= MaxInventoryWeight)
            {
                aviableItems.Add(item);
                CurrentItemIndex = Mathf.Clamp(CurrentItemIndex, 0, aviableItems.Count - 1);
                OnItemAdded?.Invoke();
                return true;
            }
            return false;
        }
        public bool TryRemoveItem(int index)
        {
            if (index < 0 || index >= aviableItems.Count || !aviableItems[index].data.Dropable)
                return false;

            aviableItems.RemoveAt(index);
            CurrentItemIndex = Mathf.Clamp(CurrentItemIndex, 0, aviableItems.Count - 1);
            OnItemRemoved?.Invoke();
            return true;
        }
    }
}