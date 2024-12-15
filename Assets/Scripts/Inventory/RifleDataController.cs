using Assets.Scripts.Entities;
using Assets.Scripts.Inventory.DynamicData;
using Assets.Scripts.Inventory.Scriptables;
using UnityEngine;

namespace Assets.Scripts.Inventory
{
    public class RifleDataController : WeaponDataController
    {
        [SerializeField] private Transform raycastPos;

        private void Update()
        {
            if (data is DynamicWeaponData weaponData && data.data is BaseWeaponData baseWeaponData)
            {
                if (baseWeaponData.MyType == BaseWeaponData.WeaponType.Rifle)
                {
                    if (weaponData.reloadTime > 0f && weaponData.overallAmmo > 0)
                    {
                        weaponData.reloadTime -= Time.deltaTime;

                        if (weaponData.reloadTime <= 0f)
                        {
                            handsAnimator.SetBool("Reload", false);
                            itemAnimator.SetBool("Reload", false);

                            int v = weaponData.overallAmmo;
                            weaponData.overallAmmo -= Mathf.Clamp(baseWeaponData.BaseAmmo - weaponData.currentAmmo, 0, weaponData.overallAmmo);
                            weaponData.currentAmmo += Mathf.Clamp(baseWeaponData.BaseAmmo - weaponData.currentAmmo, 0, v);
                            AmmoChangeEvent();
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.R) && weaponData.currentAmmo != weaponData.overallAmmo && weaponData.overallAmmo > 0 && weaponData.reloadTime <= 0f)
                        HandleReload(weaponData, baseWeaponData);

                    HandleFire(weaponData, baseWeaponData);
                }
                else if (baseWeaponData.MyType == BaseWeaponData.WeaponType.Melee)
                    HandleFire(weaponData, baseWeaponData);
            }
        }
        private void HandleFire(DynamicWeaponData data, BaseWeaponData weaponData)
        {
            if (Input.GetMouseButton(0) && data.fireTime <= Time.time && data.currentAmmo > 0)
            {
                handsAnimator.SetBool("Fire", true);
                itemAnimator.SetBool("Fire", true);

                data.fireTime = Time.time + 1f / weaponData.FireRate;
                data.currentAmmo--;

                ThrowDamage(weaponData, raycastPos);

                if (data.currentAmmo <= 0)
                    HandleReload(data, weaponData);

                AmmoChangeEvent();
            }
            else if (!Input.GetMouseButton(0))
            {
                handsAnimator.SetBool("Fire", false);
                itemAnimator.SetBool("Fire", false);
            }
        }
        private void HandleReload(DynamicWeaponData data, BaseWeaponData weaponData)
        {
            data.reloadTime = weaponData.ReloadDuration;
            handsAnimator.SetBool("Reload", true);
            itemAnimator.SetBool("Reload", true);
            ReloadEvent();
            AmmoChangeEvent();
        }
    }
}