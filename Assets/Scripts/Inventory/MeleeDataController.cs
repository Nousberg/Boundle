using System;
using System.Collections.Generic;
using Assets.Scripts.Core.Input_System;
using Assets.Scripts.Core.InputSystem;
using Assets.Scripts.Effects;
using Assets.Scripts.Network;
using Photon.Pun;
using UnityEngine;

namespace Assets.Scripts.Inventory
{
    internal class MeleeDataController : WeaponDataController
    {
        [SerializeField] private EffectContainer effects;
        [SerializeField] private Transform raycastPos;

        [Header("Additional")]
        [SerializeField] private List<string> attackAnimations = new List<string>();
        [SerializeField] private string defenceAnimation;

        private InputState inputSource;
        private Effect resist;
        private float defaultAttackAnimsSpeed;

        public void Init(InputState inputSource)
        {
            this.inputSource = inputSource;
            defaultAttackAnimsSpeed = handsAnimator.GetFloat("AttackSpeed");
        }

        private void Update()
        {
            if (weaponData != null)
            {
                if (!inputSource.BoolBinds[InputHandler.InputBind.MOUSERIGHT] && inputSource.BoolBinds[InputHandler.InputBind.MOUSELEFT] && weaponData.fireTime <= Time.time)
                {
                    if (!PhotonNetwork.OfflineMode && !Convert.ToBoolean(PhotonNetwork.CurrentRoom.CustomProperties[Connector.ROOM_HASHTABLE_ALLOW_DAMAGE_KEY]))
                        return;

                    weaponData.fireTime = Time.time + 1f / baseWeaponData.FireRate;

                    ThrowDamage(raycastPos);

                    EndAnims();
                    handsAnimator.SetBool(attackAnimations[UnityEngine.Random.Range(0, attackAnimations.Count - 1)], true);
                }
                else if (inputSource.BoolBinds[InputHandler.InputBind.MOUSERIGHT])
                    handsAnimator.SetBool(defenceAnimation, true);
                else
                {
                    effects.RemoveEffect(resist);
                    EndAnims();
                }
            }
        }
        private void OnDefenceToggled(InputHandler.InputBind bind)
        {
            if (bind == InputHandler.InputBind.MOUSERIGHT)
            {
                resist = new Resistance(carrier, 0, 100f, true);
                effects.ApplyEffect(resist);
                EndAnims();
            }
        }
        private void EndAnims()
        {
            foreach (var anim in attackAnimations)
                handsAnimator.SetBool(anim, false);

            handsAnimator.SetBool(defenceAnimation, false);
        }
        private void OnDisable() => inputSource.InputRecieved -= OnDefenceToggled;
        private void OnEnable() => inputSource.InputRecieved += OnDefenceToggled;
    }
}
