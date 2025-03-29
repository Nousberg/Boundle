using Assets.Scripts.Inventory.Scriptables;
using Assets.Scripts.Inventory.DynamicData;
using System;
using UnityEngine;

namespace Assets.Scripts.Inventory
{
    public class ItemDataController : MonoBehaviour
    {
        [SerializeField] protected Animator handsAnimator;
        [SerializeField] protected Animator itemAnimator;

        [SerializeField] protected InventoryDataController inventory { get; private set; }
        [field: SerializeField] public BaseItemData BaseData { get; private set; }

        public event Action OnStartAction;
        public event Action OnEndAction;

        public DynamicItemData GetData => data;

        protected DynamicItemData data;

        public virtual void InjectData(DynamicItemData data) => this.data = data;
        protected void OnStartActionEventTrigger() => OnStartAction?.Invoke();
        protected void OnEndActionEventTrigger() => OnEndAction?.Invoke();
    }
}