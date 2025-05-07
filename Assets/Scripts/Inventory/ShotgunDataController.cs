using Assets.Scripts.Core.Input_System;
using Assets.Scripts.Core.InputSystem;
using Photon.Pun;
using UnityEngine;
using Assets.Scripts.Network;
using System;

namespace Assets.Scripts.Inventory
{
    public class ShotgunDataController : WeaponDataController
    {
        [SerializeField] private Transform raycastPos;

        private InputState inputSource;
        private float initReloadAnimSpeed;
        private float nextAmmoIncrease;

        public void Init(InputState inputSource)
        {
            this.inputSource = inputSource;
            initReloadAnimSpeed = handsAnimator.GetFloat(ReloadAnimationSpeedName);
        }

        private void Update()
        {
            if (weaponData != null)
            {
                if (weaponData.reloadTime == 1f)
                {
                    nextAmmoIncrease -= Time.deltaTime;

                    if (weaponData.currentAmmo >= baseWeaponData.BaseAmmo || weaponData.overallAmmo <= 0)
                    {
                        weaponData.reloadTime = 0f;
                        HandleReloadEnd();
                        return;
                    }

                    if (nextAmmoIncrease <= 0f)
                    {
                        Reload();
                        nextAmmoIncrease = baseWeaponData.ReloadSpeed;
                    }
                }
                if (inputSource.BoolBinds[InputHandler.InputBind.MOUSELEFT] && weaponData.fireTime <= Time.time)
                {
                    if (!PhotonNetwork.OfflineMode && !Convert.ToBoolean(PhotonNetwork.CurrentRoom.CustomProperties[Connector.ROOM_HASHTABLE_ALLOW_DAMAGE_KEY]))
                        return;

                    weaponData.reloadTime = 0f;
                    HandleReloadEnd();

                    if (weaponData.currentAmmo < 1 && weaponData.overallAmmo > 0)
                        weaponData.reloadTime = 1f;
                    else if (weaponData.currentAmmo > 0)
                    {
                        weaponData.currentAmmo--;
                        weaponData.fireTime = Time.time + 1f / baseWeaponData.FireRate;

                        handsAnimator.SetBool(FireAnimationName, true);
                        itemAnimator.SetBool(FireAnimationName, true);

                        FireEvent();
                        ThrowDamage(raycastPos);

                        AmmoChangeEvent();
                    }
                }
                else
                {
                    handsAnimator.SetBool(FireAnimationName, false);
                    itemAnimator.SetBool(FireAnimationName, false);
                }
            }
        }

        private void OnBindToggle(InputHandler.InputBind bind)
        {
            if (bind == InputHandler.InputBind.MOUSELEFT && weaponData.currentAmmo < 1)
                OutOfAmmoEvent();
            if (bind == InputHandler.InputBind.MOUSELEFT)
                OnStartActionEventTrigger();
            if (bind == InputHandler.InputBind.RELOADTOGGLE && weaponData.overallAmmo > 0 && weaponData.currentAmmo < baseWeaponData.BaseAmmo)
                weaponData.reloadTime = 1f;
            else if (bind == InputHandler.InputBind.MOUSERIGHT && weaponData.reloadTime == 0f)
            {
                AimedEvent();
                OnStartActionEventTrigger();
            }
        }
        private void OnBindCancel(InputHandler.InputBind bind)
        {
            if (bind == InputHandler.InputBind.MOUSERIGHT)
                UnAimedEvent();
            else if (bind == InputHandler.InputBind.MOUSELEFT)
                OnEndActionEventTrigger();
            else if (bind == InputHandler.InputBind.MOUSERIGHT)
                OnEndActionEventTrigger();
        }
        private void HandleReloadEnd()
        {
            handsAnimator.SetBool(ReloadAnimationName, false);
            itemAnimator.SetBool(ReloadAnimationName, false);

            ReloadEndEvent();
        }
        private void Reload()
        {
            handsAnimator.SetBool(ReloadAnimationName, true);
            itemAnimator.SetBool(ReloadAnimationName, true);

            weaponData.overallAmmo--;
            weaponData.currentAmmo++;

            UnAimedEvent();
            ReloadEvent();
            AmmoChangeEvent();
        }
        private void OnDisable()
        {
            if (weaponData != null)
                weaponData.reloadTime = 0f;

            inputSource.InputRecieved -= OnBindToggle;
            inputSource.InputCanceled -= OnBindCancel;
        }
        private void OnEnable()
        {
            inputSource.InputRecieved += OnBindToggle;
            inputSource.InputCanceled += OnBindCancel;
        }
    }
}
