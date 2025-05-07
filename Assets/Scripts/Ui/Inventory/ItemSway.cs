using Assets.Scripts.Core.Input_System;
using Assets.Scripts.Core.InputSystem;
using Assets.Scripts.Entities;
using Assets.Scripts.Inventory;
using Assets.Scripts.Inventory.Scriptables;
using Assets.Scripts.Movement;
using UnityEngine;

namespace Assets.Scripts.Ui.Inventory
{
    public class ItemSway : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InventoryDataController inventory;
        [SerializeField] private PlayerMovementLogic movement;
        [SerializeField] private Entity player;
        [SerializeField] private Rigidbody playerRb;

        [Header("Position Properties")]
        [SerializeField] private Vector3 afterSwitchOffset;
        [SerializeField] private float swayAmount;
        [SerializeField] private float swayLerpSpeed;

        [Header("Rotation Properties")]
        [SerializeField] private float rotSwayAmount;
        [SerializeField] private float rotSwayLerpSpeed;

        [Header("Dynamic Rotation Properties")]
        [SerializeField] private Vector3 keyInfluence;
        [SerializeField] private float walkBobbingMultiplier;
        [SerializeField] private float walkBobbingFrequency;
        [SerializeField] private float runBobbingMultiplier;
        [SerializeField] private float runBobbingFrequency;
        [SerializeField] private float maxYRot;
        [SerializeField] private float runKeyBoost;
        [SerializeField] private float recoilLerpSpeed;
        [SerializeField] private float recoilMultiplier;

        private InputState inputSource;
        private float savedRecoil;
        private float recoilOffset;
        private bool aimed;
        private bool inited;
        private Vector3 aimedOffset;
        private Vector2 mouseInput;
        private Vector3 initialPos;
        private Quaternion initialRot;

        public void Init(InputState inputSource)
        {
            this.inputSource = inputSource;

            initialPos = transform.localPosition;
            initialRot = transform.localRotation;

            inventory.OnItemAdded += HandleItemChange;
            inventory.OnItemRemoved += HandleItemChange;
            inventory.OnItemSwitched += () => HandleItemChange(string.Empty);

            inited = true;
        }

        private void Update()
        {
            if (!inited)
                return;

            Vector3 mouseRot = inputSource.VectorBinds[InputHandler.InputBind.LOOK];
            Vector3 moveDir = inputSource.VectorBinds[InputHandler.InputBind.WASD];
            Vector3 bobbing = Vector3.zero;
            Vector3 targetPos = Vector3.zero;

            targetPos = (aimed ? aimedOffset : initialPos) + (-mouseRot * swayAmount);

            if (movement.IsWaking)
            {
                float frequency = (movement.IsRunning ? runBobbingFrequency : walkBobbingFrequency);
                float multiplier = (movement.IsRunning ? runBobbingMultiplier : walkBobbingMultiplier);

                bobbing.x = Mathf.Cos(Time.time * frequency / 2f) * multiplier;
                bobbing.y = Mathf.Sin(Time.time * frequency) * multiplier;

                targetPos += bobbing;
            }

            targetPos.z -= recoilOffset;

            if (movement.IsWalkingAttemp)
                targetPos -= new Vector3(moveDir.x * keyInfluence.x, 0f, moveDir.y * keyInfluence.z) * (inputSource.BoolBinds[InputHandler.InputBind.RUNSTATE] ? runKeyBoost : 1f);

            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, swayLerpSpeed * Time.deltaTime);

            Quaternion targetRot = initialRot;
            targetRot.x += -recoilOffset + Mathf.Clamp(playerRb.velocity.y * rotSwayAmount, -maxYRot, maxYRot);

            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, rotSwayLerpSpeed * Time.deltaTime);

            recoilOffset = Mathf.Lerp(recoilOffset, 0f, recoilLerpSpeed * Time.deltaTime);
        }

        private void HandleItemChange(string name)
        {
            transform.localPosition += afterSwitchOffset;

            if (inventory.GetItems[inventory.CurrentItemIndex].data is BaseWeaponData baseWeapon)
            {
                savedRecoil = baseWeapon.Recoil * recoilMultiplier;
                aimedOffset = baseWeapon.AimedPosition;

                WeaponDataController weapon = inventory.AllInGameItems.Find(n => n.BaseData.Id == baseWeapon.Id) as WeaponDataController;

                if (weapon != null)
                {
                    weapon.OnFire -= HandleRecoil;
                    weapon.OnFire += HandleRecoil;

                    weapon.OnAimed -= () => OnAimed(true);
                    weapon.OnUnAimed -= () => OnAimed(false);

                    weapon.OnAimed += () => OnAimed(true);
                    weapon.OnUnAimed += () => OnAimed(false);
                }
            }
        }

        private void HandleRecoil() => recoilOffset = savedRecoil;
        private void OnAimed(bool v) => aimed = v;
    }
}
