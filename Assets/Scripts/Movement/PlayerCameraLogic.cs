using Assets.Scripts.Entities;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerMovementLogic))]
    public class PlayerCameraLogic : MonoBehaviour
    {
        [field: Header("References")]
        [SerializeField] private Camera cam;

        [Header("Camera Properties")]
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

        [Header("Noise Properties")]
        [Min(0f)][SerializeField] private float noiseGenerationSpeed;
        [SerializeField] private float noiseLerpSpeed;
        [SerializeField] private Vector3 noiseRotation;
        [SerializeField] private Vector3 dynamicRotAmplitudeFactor;
        [SerializeField] private float dynamicRotFrequencyFactor;

        private PlayerMovementLogic playerMovement => GetComponent<PlayerMovementLogic>();
        private Entity player => GetComponent<Entity>();
        private Rigidbody rb => GetComponent<Rigidbody>();
        private float _flySpeed;
        private bool isCrouch;
        private float currentWalkSpeed;
        private float xRot;
        private float zRot;
        private float defaultFov;
        private float targetFov;
        private float _dynamicRotFreqerencyFactor;
        private float noiseRotLerpSpeed;
        private float spreadOffset;
        private Vector3 collCenter;
        private float collSize;
        private Vector3 _dynamicRotAmplitudeFactor;
        private Vector3 dynamicOffset;
        private Vector3 keyInput;
        private Vector3 mouseInput;
        private Vector3 moveVector;
        private Vector3 curNoiseRot;
        private Vector3 noiseRot;
        private Vector3 generatedNoise;
        private Quaternion targetRot;

        private void Start()
        {
            defaultFov = cam.fieldOfView;
        }
        private void Update()
        {
            keyInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            mouseInput = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0f) * Sensitivity;

            StartCoroutine("GenerateNoise");
            noiseRot = Vector3.Lerp(noiseRot, generatedNoise, noiseRotLerpSpeed * Time.deltaTime);

            xRot -= mouseInput.y + spreadOffset;
            dynamicOffset = _dynamicRotAmplitudeFactor * Mathf.Cos(Time.time * _dynamicRotFreqerencyFactor);
            zRot = Mathf.Lerp(zRot, (-mouseInput.x * zLookOffset) + (keyInput.x * zWalkLookOffset), zLookOffsetSpeed * Time.deltaTime);

            transform.Rotate(Vector3.up * mouseInput.x);
            targetRot = Quaternion.Euler(Mathf.Clamp(xRot + jumpCameraOffset * rb.velocity.y, -maxCamRotY, maxCamRotY), 0f, -zRot) * Quaternion.Euler(dynamicOffset * (playerMovement.IsWaking ? 1f : 0f)) * Quaternion.Euler(noiseRot);
            spreadOffset = 0f;

            cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, targetRot, camRotLerpSpeed * Time.deltaTime);
            cam.fieldOfView = Mathf.Clamp(Mathf.Lerp(cam.fieldOfView, defaultFov + playerMovement.CurrentVelocity * fovOffsetAmount, fovLerpSpeed * Time.deltaTime), 0f, maxFov);

            float healthAspect = Mathf.Max(player.Health / player.BaseHealth, 0.1f);
            float velocity = Mathf.Clamp(playerMovement.CurrentVelocity, 0.25f, 1f);
            noiseRotLerpSpeed = noiseLerpSpeed / healthAspect * velocity;
            _dynamicRotAmplitudeFactor = dynamicRotAmplitudeFactor / healthAspect * velocity;
            _dynamicRotFreqerencyFactor = dynamicRotFrequencyFactor / healthAspect * velocity;
            curNoiseRot = noiseRotation / healthAspect * velocity;
        }
        private IEnumerator GenerateNoise()
        {
            yield return new WaitForSeconds(1f / noiseGenerationSpeed);
            generatedNoise = new Vector3(
                Random.Range(-curNoiseRot.x, curNoiseRot.x),
                Random.Range(-curNoiseRot.y, curNoiseRot.y),
                Random.Range(-curNoiseRot.z, curNoiseRot.z));
        }
        public void Spread(int v1, int v2, float value)
        {
            spreadOffset = value;
        }
    }
}
