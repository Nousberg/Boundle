using Assets.Scripts.Inventory.Scriptables;
using Assets.Scripts.Inventory;
using Assets.Scripts.Movement;
using System.Collections;
using UnityEngine;
using Assets.Scripts.Entities;

namespace Assets.Scripts.Ui.Inventory
{
    [RequireComponent(typeof(InventoryDataController))]
    public class ItemSway : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerMovementLogic movement;
        [SerializeField] private Entity player;
        [SerializeField] private Rigidbody playerRb;

        [Header("Position Properties")]
        [SerializeField] private float maxFallSwayAmount;
        [SerializeField] private float maxSwayAmountX;
        [SerializeField] private float minSwayAmountX;
        [SerializeField] private float maxSwayAmountZ;
        [SerializeField] private float minSwayAmountZ;
        [SerializeField] private float fallSwayAmount;
        [SerializeField] private float swayAmount;
        [SerializeField] private float swayLerpSpeed;

        [Header("Rotation Properties")]
        [SerializeField] private float maxRotSwayAmount;
        [SerializeField] private float rotSwayAmount;
        [SerializeField] private float keyRotSwayAmount;
        [SerializeField] private float rotSwayLerpSpeed;

        [Header("Noise Rotation Properties")]
        [SerializeField] private Vector3 maxNoiseRotation;
        [SerializeField] private float noiseGenerationDuration;
        [SerializeField] private float noiseLerpSpeed;

        [Header("Dynamic Rotation Properties")]
        [SerializeField] private float dynamicRotAmplitude;
        [SerializeField] private float dyanmicRotFrequency;
        [SerializeField] private Vector3 dynamicRotAmount;

        private float GetShiftBonus => Input.GetKey(KeyCode.LeftShift) ? 1.5f : 1f;

        private InventoryDataController inventory => GetComponent<InventoryDataController>();

        private float savedSpread;
        private float spreadOffset;
        private Vector2 mouseInput;
        private Vector3 targetPosition;
        private Vector2 keyInput;
        private Vector3 generatedNoise;
        private Vector3 noiseRot;
        private Vector3 initialPos;
        private Quaternion targetRotation;
        private Quaternion initialRot;

        private void Start()
        {
            initialPos = transform.localPosition;
            initialRot = transform.localRotation;

            inventory.OnItemAdded += HandleItemChange;
            inventory.OnItemRemoved += HandleItemChange;
            inventory.OnItemSwitched += HandleItemChange;
        }
        private void Update()
        {
            StartCoroutine(GenerateNoise());

            Vector3 targetNoiseRot = generatedNoise;
            targetNoiseRot.y *= movement.CurrentVelocity;
            targetNoiseRot.x *= movement.CurrentVelocity;

            float maxVelocityComponent = Mathf.Max(playerRb.velocity.y, playerRb.velocity.x, playerRb.velocity.z);

            noiseRot = Vector3.Slerp(noiseRot, generatedNoise / Mathf.Max(player.Health / player.BaseHealth, 0.25f), noiseLerpSpeed * Time.deltaTime);

            mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            keyInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            float keyInputMagnitude = keyInput.magnitude;

            Vector3 localVelocity = transform.InverseTransformDirection(playerRb.velocity);

            targetPosition = initialPos + new Vector3(
                Mathf.Clamp(-mouseInput.x + localVelocity.x, minSwayAmountX, maxSwayAmountX), 
                Mathf.Clamp(-mouseInput.y, -maxSwayAmountX, maxSwayAmountX) + Mathf.Clamp(localVelocity.y * fallSwayAmount, -maxFallSwayAmount, maxFallSwayAmount), 
                Mathf.Clamp(-localVelocity.z + spreadOffset, minSwayAmountZ, maxSwayAmountZ)) * swayAmount;

            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, swayLerpSpeed * Time.deltaTime);

            targetRotation = initialRot * (Quaternion.Euler(spreadOffset, 0f, Mathf.Clamp(mouseInput.x + -localVelocity.y * keyRotSwayAmount, -maxRotSwayAmount, maxRotSwayAmount) * rotSwayAmount) * Quaternion.Euler(noiseRot));

            if (movement.IsWaking)
                targetRotation *= Quaternion.Euler(dynamicRotAmount * (Mathf.Cos(Time.time * (dyanmicRotFrequency * keyInputMagnitude * GetShiftBonus)) * dynamicRotAmplitude * keyInputMagnitude * GetShiftBonus));

            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, rotSwayLerpSpeed * Time.deltaTime);

            spreadOffset = 0f;
        }
        private IEnumerator GenerateNoise()
        {
            yield return new WaitForSeconds(noiseGenerationDuration);

            generatedNoise = new Vector3(
                Random.Range(-maxNoiseRotation.x, maxNoiseRotation.x), 
                Random.Range(-maxNoiseRotation.y, maxNoiseRotation.y),
                Random.Range(-maxNoiseRotation.z, maxNoiseRotation.z));
        }
        private void HandleItemChange()
        {
            if (inventory.GetItems[inventory.CurrentItemIndex].data is BaseWeaponData baseWeapon)
            {
                savedSpread = baseWeapon.Spread;

                WeaponDataController weapon = inventory.AllInGameItems.Find(n => n.BaseData.Id == baseWeapon.Id) as WeaponDataController;

                if (weapon != null)
                {
                    weapon.OnFire -= HandleSpread;
                    weapon.OnFire += HandleSpread;
                }
            }
        }
        private void HandleSpread()
        {
            spreadOffset = savedSpread;
        }
    }
}