using Assets.Scripts.Core.Input_System;
using Assets.Scripts.Core.InputSystem;
using Assets.Scripts.Entities;
using Assets.Scripts.Inventory;
using Assets.Scripts.Inventory.Scriptables;
using Assets.Scripts.Inventory.View;
using Assets.Scripts.Ui.Player;
using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(InventoryDataController))]
    [RequireComponent(typeof(PlayerMovementLogic))]
    public class PlayerCameraLogic : MonoBehaviour
    {
        [field: Header("References")]
        [SerializeField] private Camera cam;

        [Header("Camera Properties")]
        [SerializeField] private float keyInfluenceSpeed;
        [Min(0f)][SerializeField] private float Sensitivity;
        [SerializeField] private float zWalkLookOffset;
        [SerializeField] private float zLookOffset;
        [SerializeField] private float zLookOffsetSpeed;
        [SerializeField] private float jumpCameraOffset;
        [Min(0f)][SerializeField] private float maxFov;
        [Min(0f)][SerializeField] private float fovLerpSpeed;
        [Min(0f)][SerializeField] private float fovOffsetAmount;
        [Min(0f)][SerializeField] private float maxCamRotY;
        [Min(0f)][SerializeField] private float camRotLerpSpeed;

        [Header("Noise Rotation Properties")]
        [SerializeField] private float shiftBonus;
        [SerializeField] private float dynamicRotStabilizationSpeed;
        [Min(0f)][SerializeField] private float noiseGenerationSpeed;
        [SerializeField] private float noiseLerpSpeed;
        [SerializeField] private Vector3 noiseRotation;
        [SerializeField] private Vector3 dynamicRotAmplitudeFactor;
        [SerializeField] private float dynamicRotFrequencyFactor;
        [SerializeField] private AnimationCurve dynamicRotCurve;

        private PlayerMovementLogic playerMovement => GetComponent<PlayerMovementLogic>();
        private InventoryDataController inventory => GetComponent<InventoryDataController>();
        private Entity player => GetComponent<Entity>();
        private Rigidbody rb => GetComponent<Rigidbody>();
        private PhotonView view => GetComponent<PhotonView>();

        private float shiftPressBonus => inputSource.BoolBinds[InputHandler.InputBind.RUNSTATE] ? shiftBonus : 1f;

        private InputState inputSource;
        private float xRot;
        private float zRot;
        private float defaultFov;
        private float targetFov;
        private float _dynamicRotFreqerencyFactor;
        private float noiseRotLerpSpeed;
        private float savedRecoil;
        private float recoilOffset;
        private float healthAspect;
        private float velocity;
        private float aimedFovOffset;
        private bool aimed;
        private Vector3 collCenter;
        private Vector3 _dynamicRotAmplitudeFactor;
        private Vector3 dynamicOffset;
        private Vector3 keyInput;
        private Vector2 mouseInput;
        private Vector3 moveVector;
        private Vector3 curNoiseRot;
        private Vector3 noiseRot;
        private Vector3 generatedNoise;
        private Quaternion targetRot;
        private Transform cameraTransform;

        public void Init(InputState inputSource)
        {
            this.inputSource = inputSource;

            defaultFov = cam.fieldOfView;
            cameraTransform = cam.transform;

            inventory.OnItemAdded += HandleItemChange;
            inventory.OnItemRemoved += HandleItemChange;
            inventory.OnItemSwitched += () => HandleItemChange(string.Empty);
        }
        private void Update()
        {
            if (view.IsMine)
            {
                if (playerMovement.IsWaking)
                    _dynamicRotAmplitudeFactor = Vector3.Lerp(_dynamicRotAmplitudeFactor, dynamicRotAmplitudeFactor, dynamicRotStabilizationSpeed * Time.deltaTime);
                else
                    _dynamicRotAmplitudeFactor = Vector3.zero;

                keyInput = new Vector3(inputSource.VectorBinds[InputHandler.InputBind.WASD].x, 0f, inputSource.VectorBinds[InputHandler.InputBind.WASD].y);
                mouseInput = inputSource.VectorBinds[InputHandler.InputBind.LOOK] * Sensitivity;

                StartCoroutine("GenerateNoise");
                noiseRot = Vector3.Lerp(noiseRot, generatedNoise, noiseRotLerpSpeed * Time.deltaTime);

                xRot -= mouseInput.y + recoilOffset;

                float curveValue = dynamicRotCurve.Evaluate((Mathf.Sin(Time.time * _dynamicRotFreqerencyFactor) + 1f) / 2f);
                dynamicOffset = _dynamicRotAmplitudeFactor * curveValue * shiftPressBonus * Time.deltaTime;

                zRot = Mathf.Lerp(zRot, (-mouseInput.x * zLookOffset) + (keyInput.x * zWalkLookOffset), zLookOffsetSpeed * Time.deltaTime);

                transform.Rotate(Vector3.up * mouseInput.x);
                targetRot = Quaternion.Euler(Mathf.Clamp(xRot + jumpCameraOffset * rb.velocity.y, -maxCamRotY, maxCamRotY), 0f, -zRot) *
                            Quaternion.Euler(dynamicOffset) *
                            Quaternion.Euler(noiseRot);

                cameraTransform.localRotation = Quaternion.Slerp(cam.transform.localRotation, targetRot, camRotLerpSpeed * Time.deltaTime);
                cam.fieldOfView = Mathf.Clamp(Mathf.Lerp(cam.fieldOfView, (aimed ? aimedFovOffset : defaultFov) + playerMovement.CurrentVelocity * fovOffsetAmount, fovLerpSpeed * Time.deltaTime), 0f, maxFov);

                healthAspect = Mathf.Max(player.Health / player.BaseHealth, 0.7f);
                velocity = Mathf.Clamp(playerMovement.CurrentVelocity, 0.4f, 1f);
                noiseRotLerpSpeed = noiseLerpSpeed / healthAspect * velocity;
                _dynamicRotAmplitudeFactor = dynamicRotAmplitudeFactor / healthAspect * velocity;
                _dynamicRotFreqerencyFactor = dynamicRotFrequencyFactor / healthAspect * velocity;
                curNoiseRot = noiseRotation / healthAspect * velocity;

                recoilOffset = 0f;
            }
        }

        private IEnumerator GenerateNoise()
        {
            yield return new WaitForSeconds(1f / noiseGenerationSpeed);
            generatedNoise = new Vector3(
                Random.Range(-curNoiseRot.x, curNoiseRot.x),
                Random.Range(-curNoiseRot.y, curNoiseRot.y),
                Random.Range(-curNoiseRot.z, curNoiseRot.z));
        }

        private void HandleItemChange(string name)
        {
            if (inventory.GetItems[inventory.CurrentItemIndex].data is BaseWeaponData baseWeapon)
            {
                savedRecoil = baseWeapon.Recoil;
                aimedFovOffset = baseWeapon.AimedFovChange;

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
        private void OnAimed(bool v) => aimed = v;
        private void HandleRecoil() => recoilOffset = savedRecoil;
    }
}
