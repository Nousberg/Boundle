using Assets.Scripts.Inventory.Scriptables;
using Assets.Scripts.Inventory.DynamicData;
using System;
using UnityEngine;
using Assets.Scripts.Effects;

namespace Assets.Scripts.Inventory
{
    public class ItemDataController : MonoBehaviour
    {
        [SerializeField] protected Animator handsAnimator;
        [SerializeField] protected Animator itemAnimator;

        [SerializeField] protected InventoryDataController inventory { get; private set; }
        [field: SerializeField] public BaseItemData BaseData { get; private set; }

        public DynamicItemData GetData => data;

        protected DynamicItemData data;

        public virtual void InjectData(DynamicItemData data) => this.data = data;
    }
}