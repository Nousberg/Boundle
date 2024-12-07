using Assets.Scripts.Entities;
using System;
using UnityEngine;

namespace Assets.Scripts.Inventory
{
    public class WeaponDataController : ItemDataController
    {
        [field: SerializeField] protected Entity carrier { get; private set; }

        public delegate void FireHandler(ref float damage);

        public event FireHandler OnFire;
        public event Action OnReload;

        protected void FireEvent(ref float damage) => OnFire?.Invoke(ref damage);
        protected void ReloadEvent() => OnReload?.Invoke();
    }
}