using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Effects;
using Assets.Scripts.Movement.Scriptables;
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
        [Header("References")]
        [SerializeField] private Transform feets;
        [SerializeField] private Camera cam;

        [field: Header("Movement")]
        [SerializeField] private MovementData movementData;
        [Min(0f)][SerializeField] public float feetLength;
        [Min(0f)][SerializeField] public float flyLerpSpeed;

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
        private EffectContainer effects => GetComponent<EffectContainer>();
        private Entity player => GetComponent<Entity>();
        private float _flySpeed;
        private float _runSpeedBoost;
        private float _jumpPower;
        private bool isCrouch;
        private float currentWalkSpeed;
        private Vector3 collCenter;
        private float collSize;
        private Vector3 keyInput;
        private Vector3 mouseInput;
        private Vector3 moveVector;

        private void Start()
        {
            collSize = coll.height;
            collCenter = coll.center;

            _jumpPower = movementData.JumpPower;
            _runSpeedBoost = movementData.RunSpeedBoost;
            currentWalkSpeed = movementData.WalkSpeed;
            _flySpeed = movementData.FlySpeed;
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

                rb.velocity = Vector3.Lerp(rb.velocity, moveDirection * (Input.GetKey(KeyCode.LeftShift) ? _flySpeed * _runSpeedBoost : _flySpeed), flyLerpSpeed);
            }
            else if (!isFlying && IsGrounded && keyInput.sqrMagnitude > 0f)
            {
                effects.CalculateMovement(movementData, ref currentWalkSpeed, ref _flySpeed, ref _runSpeedBoost, ref _jumpPower);

                moveVector = transform.TransformDirection(keyInput) *
                    (Input.GetKey(KeyCode.LeftShift) ? currentWalkSpeed * _runSpeedBoost : currentWalkSpeed);
                rb.velocity = new Vector3(moveVector.x, rb.velocity.y, moveVector.z);
            }

            CurrentVelocity = rb.velocity.magnitude;
        }

        private void Update()
        {
            keyInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

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

            RaycastHit hit;
            IsGrounded = Physics.Raycast(feets.position, -feets.up, out hit, feetLength);

            if (Input.GetKeyDown(KeyCode.Space) && (IsGrounded || isFlying))
            {
                rb.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
            }
        }
        public void ToggleFly(bool state)
        {
            rb.useGravity = !state;
            isFlying = state;
        }
    }
}