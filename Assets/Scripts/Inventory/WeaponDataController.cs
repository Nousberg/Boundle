using Assets.Scripts.Core.Environment;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Liquids;
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

        protected void ThrowDamage(Transform raycastPos)
        {
            float damage = baseWeaponData.Damage;
            FireEffectsEvent(ref damage);

            for (int i = 0; i <= baseWeaponData.BulletsPerShot; i++)
            {
                RaycastHit hit;
                Vector3 recoilAdjustment = new Vector3(
                UnityEngine.Random.Range(-baseWeaponData.Spread, baseWeaponData.Spread),
                UnityEngine.Random.Range(-baseWeaponData.Spread, baseWeaponData.Spread),
                UnityEngine.Random.Range(-baseWeaponData.Spread, baseWeaponData.Spread)
);

                if (Physics.Raycast(raycastPos.position, raycastPos.forward + recoilAdjustment, out hit, baseWeaponData.Range))
                {
                    Entity target = hit.collider.GetComponent<Entity>();

                    //if (target != null)
                    //{
                    //    float damageModifier = 1f;
                    //
                    //    if (hit.collider.TryGetComponent<LiquidContainer>(out var liquids))
                    //    {
                    //        foreach (var liquid in liquids.GetLiquids)
                    //            switch (liquid.type)
                    //            {
                    //                case LiquidContainer.LiquidType.Water:
                    //                    damageModifier *= 0.75f;
                    //                    break;
                    //                case LiquidContainer.LiquidType.Acid:
                    //                    damageModifier *= 0.25f;
                    //                    break;
                    //            }
                    //    }
                    //
                    //    target.TakeDamage(damageModifier * damage /
                    //        Mathf.Clamp(
                    //            Mathf.Abs(
                    //                Mathf.Clamp(
                    //                    baseWeaponData.Range -
                    //                    Vector3.Distance(transform.position, hit.transform.position),
                    //                    1f,
                    //                    float.PositiveInfinity) / baseWeaponData.Range),
                    //        1f,
                    //        float.PositiveInfinity),
                    //    carrier,
                    //    baseWeaponData.DamageType);
                    //}

                    if (hit.collider.TryGetComponent<Rigidbody>(out var rb))
                        rb.AddForce(-hit.normal * baseWeaponData.PushStrenght);
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