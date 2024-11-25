using Assets.Scripts.Entities;
using Assets.Scripts.Inventory.Scriptables;
using System;
using UnityEngine;

namespace Assets.Scripts.Inventory
{
    public class WeaponDataController : ItemDataController
    {
        [SerializeField] private Entity carrier;
        [SerializeField] private Transform raycastPos;

        public delegate void FireHandler(ref float damage);

        public event FireHandler OnFire;
        public event Action OnReload;

        private void Update()
        {
            if (data is DynamicWeaponData weaponData && data.data is BaseWeaponData baseWeaponData)
            {
                if (baseWeaponData.MyType == BaseWeaponData.WeaponType.Rifle)
                {
                    if (weaponData.reloadTime > 0f)
                    {
                        weaponData.reloadTime -= Time.deltaTime;

                        if (weaponData.reloadTime <= 0f)
                        {
                            int v = weaponData.overallAmmo;
                            weaponData.overallAmmo -= Mathf.Clamp(baseWeaponData.BaseAmmo - weaponData.currentAmmo, 0, weaponData.overallAmmo);
                            weaponData.currentAmmo += Mathf.Clamp(baseWeaponData.BaseAmmo - weaponData.currentAmmo, 0, v);
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.R) && weaponData.currentAmmo != weaponData.overallAmmo && weaponData.overallAmmo > 0 && weaponData.reloadTime <= 0f)
                    {
                        weaponData.reloadTime = baseWeaponData.ReloadDuration;
                        OnReload?.Invoke();
                    }
                    if (Input.GetMouseButton(0) && weaponData.fireTime <= Time.time)
                    {
                        weaponData.fireTime = Time.time + 1f / baseWeaponData.FireRate;

                        ThrowDamage(weaponData, baseWeaponData);
                    }
                }
                else if (baseWeaponData.MyType == BaseWeaponData.WeaponType.Melee)
                {
                    if (Input.GetMouseButton(0) && weaponData.fireTime <= Time.time)
                    {
                        weaponData.fireTime = Time.time + 1f / baseWeaponData.FireRate;

                        ThrowDamage(weaponData, baseWeaponData);
                    }
                }
            }
        }
        private void ThrowDamage(DynamicItemData data, BaseWeaponData weaponData)
        {
            float damage = weaponData.Damage;
            OnFire?.Invoke(ref damage);

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
}