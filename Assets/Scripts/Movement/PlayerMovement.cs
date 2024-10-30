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

        public bool lockedDown;

        private Rigidbody rb => GetComponent<Rigidbody>();
        private CapsuleCollider coll => GetComponent<CapsuleCollider>();
        private EffectContainer effects => GetComponent<EffectContainer>();
        private Entity player => GetComponent<Entity>();
        private float flySpeed;
        private float runSpeedBoost;
        private float jumpPower;
        private bool isCrouch;
        private float currentWalkSpeed;
        private float collSize;
        private float healthAspect;
        private float FoodAspect;
        private float currentFlyLerpSpeed;
        private Vector3 collCenter;
        private Vector3 keyInput;
        private Vector3 mouseInput;
        private Vector3 moveVector;

        private void Start()
        {
            collSize = coll.height;
            collCenter = coll.center;

            jumpPower = movementData.JumpPower;
            runSpeedBoost = movementData.RunSpeedBoost;
            currentWalkSpeed = movementData.WalkSpeed;
            currentFlyLerpSpeed = flyLerpSpeed;
            flySpeed = movementData.FlySpeed;
        }
        private void FixedUpdate()
        {
            if (isFlying)
            {
                if (!lockedDown)
                {
                    Vector3 cameraForward = cam.transform.forward;
                    Vector3 cameraRight = cam.transform.right;

                    cameraForward = Vector3.Normalize(cameraForward);
                    cameraRight = Vector3.Normalize(cameraRight);

                    Vector3 moveDirection = cameraForward * keyInput.z + cameraRight * keyInput.x;

                    rb.velocity = Vector3.Lerp(rb.velocity, moveDirection * (Input.GetKey(KeyCode.LeftShift) ? flySpeed * runSpeedBoost : flySpeed), currentFlyLerpSpeed);
                }
                else
                {
                    rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, currentFlyLerpSpeed);
                }
            }
            else if (!isFlying && IsGrounded && keyInput.sqrMagnitude > 0f)
            {
                if (!lockedDown)
                {
                    if (!effects.CalculateMovement(movementData, ref currentWalkSpeed, ref flySpeed, ref runSpeedBoost, ref jumpPower))
                    {
                        jumpPower = movementData.JumpPower;
                        runSpeedBoost = movementData.RunSpeedBoost;
                        currentWalkSpeed = movementData.WalkSpeed;
                        flySpeed = movementData.FlySpeed;
                    }

                    moveVector = transform.TransformDirection(keyInput) *
                        (Input.GetKey(KeyCode.LeftShift) ? currentWalkSpeed * runSpeedBoost : currentWalkSpeed);
                    rb.velocity = new Vector3(moveVector.x, rb.velocity.y, moveVector.z);
                }
                else
                {
                    rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
                }
            }

            if (!lockedDown)
            {
                CurrentVelocity = rb.velocity.magnitude;

                healthAspect = Mathf.Max(player.Health / player.BaseHealth, 0.35f);
                FoodAspect = Mathf.Max(player.Food / player.BaseFood, 0.5f);
                currentWalkSpeed = movementData.WalkSpeed * healthAspect * FoodAspect;
                flySpeed = movementData.FlySpeed * healthAspect * FoodAspect;
                jumpPower = movementData.JumpPower * healthAspect * FoodAspect;
                runSpeedBoost = Mathf.Max(movementData.RunSpeedBoost * healthAspect * FoodAspect, 1f);
                currentFlyLerpSpeed = flyLerpSpeed * healthAspect;
            }
        }

        private void Update()
        {
            if (!lockedDown)
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
                    rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
                }
            }
        }
        public void ToggleFly(bool state)
        {
            rb.useGravity = !state;
            isFlying = state;
        }
    }
}