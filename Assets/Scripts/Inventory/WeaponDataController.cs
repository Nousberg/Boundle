using Assets.Scripts.Entities;
using Assets.Scripts.Inventory.DynamicData;
using Assets.Scripts.Inventory.Scriptables;
using System;
using UnityEngine;

namespace Assets.Scripts.Inventory
{
    public abstract class WeaponDataController : ItemDataController
    {
        [field: SerializeField] protected Entity carrier { get; private set; }


        [field: Header("Properties")]
        [field: SerializeField] protected string ReloadAnimationSpeedName { get; private set; }
        [field: SerializeField] protected string ReloadAnimationName { get; private set; }
        [field: SerializeField] protected string FireAnimationName { get; private set; }

        public delegate void FireHandler(ref float damage);

        public event FireHandler OnFireForEffects;
        public event Action OnFire;
        public event Action OnAmmoChanged;
        public event Action OnReload;

        protected DynamicWeaponData weaponData;
        protected BaseWeaponData baseWeaponData;

        protected void FireEffectsEvent(ref float damage) => OnFireForEffects?.Invoke(ref damage);
        protected void FireEvent() => OnFire?.Invoke();
        protected void AmmoChangeEvent() => OnAmmoChanged?.Invoke();
        protected void ReloadEvent() => OnReload?.Invoke();

        protected void ThrowDamage(BaseWeaponData weaponData, Transform raycastPos)
        {
            float damage = weaponData.Damage;
            FireEffectsEvent(ref damage);

            for (int i = 0; i <= weaponData.BulletsPerShot; i++)
            {
                RaycastHit hit;
                if (Physics.Raycast(raycastPos.position, raycastPos.forward, out hit, weaponData.Range))
                {
                    Entity target = hit.collider.GetComponent<Entity>();

                    if (target != null)
                        target.TakeDamage(damage /
                            Mathf.Clamp(
                                Mathf.Abs(
                                    Mathf.Clamp(
                                        weaponData.Range -
                                        Vector3.Distance(transform.position, hit.transform.position),
                                        1f,
                                        float.PositiveInfinity) / weaponData.Range),
                            1f,
                            float.PositiveInfinity),
                        carrier,
                        weaponData.DamageType);
                }
            }
        }

        public override void InjectData(DynamicItemData data)
        {
            if (data.data.Id == BaseData.Id && data is DynamicWeaponData weaponData && data.data is BaseWeaponData baseData)
            {
                this.weaponData = weaponData;
                baseWeaponData = baseData;
            }
        }
    }
}