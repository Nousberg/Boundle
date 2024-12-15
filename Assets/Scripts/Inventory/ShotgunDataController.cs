using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts.Inventory
{
    public class ShotgunDataController : WeaponDataController
    {
        [SerializeField] private Transform raycastPos;

        private float initReloadAnimSpeed;
        private float nextAmmoIncrease;

        private void Start()
        {
            initReloadAnimSpeed = handsAnimator.GetFloat(ReloadAnimationSpeedName);
        }
        private void Update()
        {
            if (weaponData != null)
            {
                if (weaponData.reloadTime > 0 && nextAmmoIncrease <= Time.time)
                {
                    weaponData.overallAmmo = Mathf.Max(weaponData.overallAmmo, 0);
                    weaponData.currentAmmo = Mathf.Min(weaponData.currentAmmo + 1, baseWeaponData.BaseAmmo);

                    if (weaponData.overallAmmo == 0)
                        weaponData.reloadTime = 0f;
                    else if (weaponData.currentAmmo == weaponData.overallAmmo)
                        weaponData.reloadTime = 0f;

                    nextAmmoIncrease = Time.time + baseWeaponData.ReloadDuration / Mathf.Max(weaponData.overallAmmo, 1);

                    AmmoChangeEvent();
                    HandleReloadEnd();
                }
                else if (weaponData.reloadTime > 0)
                {
                    weaponData.reloadTime -= Time.deltaTime;
                    HandleReloadEnd();
                }

                if (Input.GetMouseButtonDown(0) && weaponData.fireTime <= Time.time && weaponData.currentAmmo > 0)
                {
                    handsAnimator.SetBool(FireAnimationName, true);
                    itemAnimator.SetBool(FireAnimationName, true);

                    weaponData.fireTime = Time.time + 1f / baseWeaponData.FireRate;
                    weaponData.currentAmmo--;

                    ThrowDamage(baseWeaponData, raycastPos);

                    if (weaponData.currentAmmo <= 0)
                        HandleReload();

                    AmmoChangeEvent();
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    weaponData.reloadTime = 0f;
                    HandleReloadEnd();
                }
                else if (Input.GetKeyDown(KeyCode.R) && weaponData.currentAmmo != weaponData.overallAmmo && weaponData.overallAmmo > 0 && weaponData.reloadTime <= 0f)
                    HandleReload();
                else if (!Input.GetMouseButtonDown(0))
                {
                    handsAnimator.SetBool(FireAnimationName, false);
                    itemAnimator.SetBool(FireAnimationName, false);
                }
            }
        }
        private void HandleReloadEnd()
        {
            if (weaponData.reloadTime <= 0)
            {
                handsAnimator.SetBool(ReloadAnimationName, false);
                itemAnimator.SetBool(ReloadAnimationName, false);

                AmmoChangeEvent();
                ReloadEvent();
            }
        }
        private void HandleReload()
        {
            weaponData.reloadTime = baseWeaponData.ReloadDuration;
            handsAnimator.SetBool(ReloadAnimationName, true);
            itemAnimator.SetBool(ReloadAnimationName, true);

            handsAnimator.SetFloat(ReloadAnimationSpeedName, initReloadAnimSpeed * (Mathf.Clamp(weaponData.overallAmmo, 1, 2) / 2f));
            ReloadEvent();
            AmmoChangeEvent();
        }
    }
}