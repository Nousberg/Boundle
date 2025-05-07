using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Liquids;
using Assets.Scripts.Inventory.DynamicData;
using Assets.Scripts.Inventory.Scriptables;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Inventory
{
    public abstract class WeaponDataController : ItemDataController
    {
        private Dictionary<LiquidContainer.LiquidType, float> bulletSlowdownValueByLiquid = new Dictionary<LiquidContainer.LiquidType, float>()
        { 
            { LiquidContainer.LiquidType.Water, 0.55f },
            { LiquidContainer.LiquidType.Acid, 0.25f },
        };

        [field: SerializeField] protected Entity carrier { get; private set; }


        [field: Header("Properties")]
        [field: SerializeField] protected string ReloadAnimationSpeedName { get; private set; }
        [field: SerializeField] protected string ReloadAnimationName { get; private set; }
        [field: SerializeField] protected string FireAnimationName { get; private set; }

        public delegate void FireHandler(ref float damage);

        public event FireHandler OnFireForEffects;
        public event Action OnAimed;
        public event Action OnUnAimed;
        public event Action OnOutOfAmmo;
        public event Action OnFire;
        public event Action OnAmmoChanged;
        public event Action OnReload;
        public event Action OnReloadEnd;

        protected DynamicWeaponData weaponData;
        protected BaseWeaponData baseWeaponData;

        protected void FireEffectsEvent(ref float damage) => OnFireForEffects?.Invoke(ref damage);
        protected void FireEvent() => OnFire?.Invoke();
        protected void OutOfAmmoEvent() => OnOutOfAmmo?.Invoke();
        protected void AmmoChangeEvent() => OnAmmoChanged?.Invoke();
        protected void ReloadEvent() => OnReload?.Invoke();
        protected void ReloadEndEvent() => OnReloadEnd?.Invoke();
        protected void AimedEvent() => OnAimed?.Invoke();
        protected void UnAimedEvent() => OnUnAimed?.Invoke();

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

                if (Physics.Raycast(raycastPos.position, raycastPos.forward + recoilAdjustment, out hit, baseWeaponData.Range, ~0, QueryTriggerInteraction.Collide))
                {
                    Entity target = hit.collider.GetComponent<Entity>();

                    if (target != null)
                    {
                        if (hit.collider.TryGetComponent<LiquidContainer>(out var liquids))
                        {
                            foreach (var liquid in liquids.GetLiquids)
                            { 
                                float liquidWeight = liquid.amount / liquids.Weight;
                                damage *= bulletSlowdownValueByLiquid[liquid.type] * liquidWeight;
                            }
                        }
                    
                        target.TakeDamage(damage /
                            Mathf.Clamp(
                                Mathf.Abs(
                                    Mathf.Clamp(
                                        baseWeaponData.Range -
                                        Vector3.Distance(transform.position, hit.transform.position),
                                        1f,
                                        float.PositiveInfinity) / baseWeaponData.Range),
                            1f,
                            float.PositiveInfinity),
                        carrier,
                        baseWeaponData.DamageType);
                    }

                    if (hit.collider.TryGetComponent<PhotonRigidbodyView>(out var rbV))
                        rbV.photonView.RPC(nameof(PhotonRigidbodyView.AddForce), RpcTarget.All, -hit.normal * baseWeaponData.PushStrenght);
                    else if (hit.collider.TryGetComponent<Rigidbody>(out var rb))
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