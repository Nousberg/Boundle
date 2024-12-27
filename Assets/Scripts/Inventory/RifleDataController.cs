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
            if (weaponData != null)
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
                    HandleReload();

                HandleFire();
            }
        }
        private void HandleFire()
        {
            if (Input.GetMouseButton(0) && weaponData.fireTime <= Time.time && weaponData.currentAmmo > 0)
            {
                handsAnimator.SetBool("Fire", true);
                itemAnimator.SetBool("Fire", true);

                weaponData.fireTime = Time.time + 1f / baseWeaponData.FireRate;

                weaponData.currentAmmo--;
                AmmoChangeEvent();

                ThrowDamage(raycastPos);
                FireEvent();
            }
            else if (!Input.GetMouseButton(0))
            {
                handsAnimator.SetBool("Fire", false);
                itemAnimator.SetBool("Fire", false);
            }
        }
        private void HandleReload()
        {
            weaponData.reloadTime = baseWeaponData.ReloadDuration;
            handsAnimator.SetBool("Reload", true);
            itemAnimator.SetBool("Reload", true);

            ReloadEvent();
            AmmoChangeEvent();
        }
    }
}