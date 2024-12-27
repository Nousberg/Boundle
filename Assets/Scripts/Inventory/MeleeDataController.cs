using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Inventory
{
    internal class MeleeDataController : WeaponDataController
    {
        [SerializeField] private Transform raycastPos;

        [Header("Additional")]
        [SerializeField] private List<string> attackAnimations = new List<string>();
        [SerializeField] private string defenceAnimation;

        private float defaultAttackAnimsSpeed;

        private void Start()
        {
            defaultAttackAnimsSpeed = handsAnimator.GetFloat("AttackSpeed");
        }
        private void Update()
        {
            if (weaponData != null)
            {
                if (Input.GetMouseButton(0) && weaponData.fireTime <= Time.time)
                {
                    //handsAnimator.SetFloat("AttackSpeed", defaultAttackAnimsSpeed * (carrier.Health / carrier.BaseHealth));

                    weaponData.fireTime = Time.time + 1f / baseWeaponData.FireRate;

                    ThrowDamage(raycastPos);

                    EndAttackAnims();
                    handsAnimator.SetBool(attackAnimations[Random.Range(0, attackAnimations.Count - 1)], true);
                }
                else
                    EndAttackAnims();
            }
        }
        private void EndAttackAnims()
        {
            foreach (var anim in attackAnimations)
                handsAnimator.SetBool(anim, false);
        }
    }
}
