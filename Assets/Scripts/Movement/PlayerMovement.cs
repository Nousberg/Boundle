using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Effects;
using Assets.Scripts.Movement.Scriptables;
using Assets.Scripts.Ui.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Movement
{
    [RequireComponent(typeof(Entity))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(TouchscreenJoystick))]
    public class PlayerMovementLogic : MovementController
    {
        [Header("References")]
        [SerializeField] private Transform feets;
        [SerializeField] private Camera cam;

        [field: Header("Movement")]
        [Min(0f)][SerializeField] private float feetLength;
        [Min(0f)] [SerializeField] private float lockedMoveLerpSpeed;
        [Min(0f)][SerializeField] private float flyLerpSpeed;

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
        private TouchscreenJoystick joystick => GetComponent<TouchscreenJoystick>();
        private Entity player => GetComponent<Entity>();
        private float collSize;
        private float healthAspect;
        private float currentFlyLerpSpeed;
        private Vector3 collCenter;
        private Vector3 keyInput;
        private Vector3 moveVector;
        private Transform camTransform;

        private void Start()
        {
            collSize = coll.height;
            collCenter = coll.center;

            ResetMovement();

            currentFlyLerpSpeed = flyLerpSpeed;
            camTransform = cam.transform;
        }
        private void FixedUpdate()
        {
            if (isFlying)
            {
                if (keyInput.sqrMagnitude > 0f && !GameVisualManager.BlockedKeyboard)
                {
                    ResetMovement();
                    Move();

                    Vector3 cameraForward = camTransform.forward;
                    Vector3 cameraRight = camTransform.right;

                    cameraForward = Vector3.Normalize(cameraForward);
                    cameraRight = Vector3.Normalize(cameraRight);

                    Vector3 moveDirection = cameraForward * keyInput.z + cameraRight * keyInput.x;

                    rb.velocity = Vector3.Lerp(rb.velocity, moveDirection * (Input.GetKey(KeyCode.LeftShift) || joystick.CriticalDistance ? flySpeed * runSpeedBoost : flySpeed), currentFlyLerpSpeed);
                }
                else
                {
                    rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, currentFlyLerpSpeed);
                }
            }
            else if (!isFlying && IsGrounded && keyInput.sqrMagnitude > 0f)
            {
                if (!GameVisualManager.BlockedKeyboard)
                {
                    ResetMovement();
                    Move();

                    moveVector = transform.TransformDirection(keyInput) *
                        (Input.GetKey(KeyCode.LeftShift) || joystick.CriticalDistance ? currentWalkSpeed * runSpeedBoost : currentWalkSpeed);
                    moveVector.y = rb.velocity.y;

                    rb.velocity = moveVector;
                }
                else
                    moveVector = Vector3.zero;
            }

            if (!GameVisualManager.BlockedKeyboard)
            {
                CurrentVelocity = rb.velocity.magnitude;

                healthAspect = Mathf.Max(player.Health / player.BaseHealth, 0.45f);
                currentWalkSpeed = data.WalkSpeed * healthAspect;
                flySpeed = data.FlySpeed * healthAspect;
                jumpPower = data.JumpPower * healthAspect;
                runSpeedBoost = Mathf.Max(data.RunSpeedBoost * healthAspect, 1f);
                currentFlyLerpSpeed = flyLerpSpeed * healthAspect;
            }
        }
        private void Update()
        {
            if (!GameVisualManager.BlockedKeyboard)
            {
                keyInput = Application.platform == RuntimePlatform.Android && joystick.joystickInput.sqrMagnitude > 0f ? joystick.joystickInput : new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

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
        private void ResetMovement()
        {
            currentWalkSpeed = data.WalkSpeed;
            runSpeedBoost = data.RunSpeedBoost;
            flySpeed = data.FlySpeed;
            jumpPower = data.JumpPower;
        }
        public void ToggleFly(bool state)
        {
            rb.useGravity = !state;
            isFlying = state;
        }
    }
}