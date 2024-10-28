using Assets.Scripts.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    [RequireComponent(typeof(Entity))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerMovementLogic : MonoBehaviour
    {
        [field: Header("References")]
        [SerializeField] private Transform feets;
        [SerializeField] private Camera cam;

        [Header("Movement")]
        [Min(0f)][SerializeField] private float FeetLength;
        [Min(0f)][SerializeField] private float jumpPower;
        [Min(0f)][SerializeField] private float walkSpeed;
        [Min(0f)][SerializeField] private float runSpeedBoost;
        [Min(0f)][SerializeField] private float flySpeed;
        [Min(0f)][SerializeField] private float flyLerpSpeed;
    
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

        [field: Header("Noise Properties")]
        [Min(0f)][SerializeField] private float noiseGenerationSpeed;
        [field: SerializeField] public float NoiseLerpSpeed { get; private set; }
        [field: SerializeField] public Vector3 NoiseRotation { get; private set; }
        [field: SerializeField] public Vector3 DynamicRotAmplitudeFactor { get; private set; }
        [field: SerializeField] public float DynamicRotFreqerencyFactor { get; private set; }
    
        public float CurrentVelocity { get; private set; }
        public bool isFlying { get; private set; }
        public bool IsGrounded { get; private set; }
        public bool IsWaking
        {
            get
            {
                return keyInput.sqrMagnitude > 0f && IsGrounded;
            }
        }
    
        private Rigidbody rb => GetComponent<Rigidbody>();
        private CapsuleCollider coll => GetComponent<CapsuleCollider>();
        private Entity player => GetComponent<Entity>();
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
            collSize = coll.height;
            collCenter = coll.center;
            currentWalkSpeed = walkSpeed;
            _flySpeed = flySpeed;
            defaultFov = cam.fieldOfView;
        }
        private void FixedUpdate()
        {
            if (isFlying)
            {
                Vector3 cameraForward = cam.transform.forward;
                Vector3 cameraRight = cam.transform.right;
    
                cameraForward = Vector3.Normalize(cameraForward);
                cameraRight = Vector3.Normalize(cameraRight);
    
                Vector3 moveDirection = cameraForward * keyInput.z + cameraRight * keyInput.x;
    
                rb.velocity = Vector3.Lerp(rb.velocity, moveDirection * (Input.GetKey(KeyCode.LeftShift) ? _flySpeed * runSpeedBoost : _flySpeed), flyLerpSpeed);
            }
            else if (!isFlying && IsGrounded && keyInput.sqrMagnitude > 0f)
            {
                moveVector = transform.TransformDirection(keyInput) *
                    (Input.GetKey(KeyCode.LeftShift) ? currentWalkSpeed * runSpeedBoost : currentWalkSpeed);
                rb.velocity = new Vector3(moveVector.x, rb.velocity.y, moveVector.z);
            }
    
            CurrentVelocity = rb.velocity.magnitude;

            float healthAspect = Mathf.Min(player.Health / player.BaseHealth, 0.1f);
            float velocity = Mathf.Clamp(CurrentVelocity, 0.5f, 1f);
            noiseRotLerpSpeed = NoiseLerpSpeed / healthAspect * velocity;
            _dynamicRotAmplitudeFactor = DynamicRotAmplitudeFactor / healthAspect * velocity;
            _dynamicRotFreqerencyFactor = DynamicRotFreqerencyFactor / healthAspect * velocity;
            curNoiseRot = NoiseRotation / healthAspect * velocity;
        }
    
        private void Update()
        {
            keyInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            mouseInput = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0f) * Sensitivity;
    
            if (Input.GetKey(KeyCode.LeftControl))
            {
                coll.center = new Vector3(0f, -0.25f, 0f);
                coll.height = 1.5f;
                isCrouch = !isCrouch;
            }
            else
            {
                coll.center = collCenter;
                coll.height = collSize;
            }
    
            StartCoroutine("GenerateNoise");
            noiseRot = Vector3.Lerp(noiseRot, generatedNoise, noiseRotLerpSpeed * Time.deltaTime);
    
            xRot -= mouseInput.y + spreadOffset;
            dynamicOffset = _dynamicRotAmplitudeFactor * Mathf.Cos(Time.time * _dynamicRotFreqerencyFactor);
            zRot = Mathf.Lerp(zRot, (-mouseInput.x * zLookOffset) + (keyInput.x * zWalkLookOffset), zLookOffsetSpeed * Time.deltaTime);
    
            transform.Rotate(Vector3.up * mouseInput.x);
            targetRot = Quaternion.Euler(Mathf.Clamp(xRot + jumpCameraOffset * rb.velocity.y, -maxCamRotY, maxCamRotY), 0f, -zRot) * Quaternion.Euler(dynamicOffset * (IsWaking ? 1f : 0f)) * Quaternion.Euler(noiseRot);
            spreadOffset = 0f;
    
            RaycastHit hit;
            IsGrounded = Physics.Raycast(feets.position, -feets.up, out hit, FeetLength);
    
            if (Input.GetKeyDown(KeyCode.Space) && (IsGrounded || isFlying))
            {
                rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            }
    
            cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, targetRot, camRotLerpSpeed * Time.deltaTime);
            cam.fieldOfView = Mathf.Clamp(Mathf.Lerp(cam.fieldOfView, defaultFov + CurrentVelocity * fovOffsetAmount, fovLerpSpeed * Time.deltaTime), 0f, maxFov);
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
        public void ToggleFly(bool state)
        {
            rb.useGravity = !state;
            isFlying = state;
        }
    }
}