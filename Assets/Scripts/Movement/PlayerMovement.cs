using Assets.Scripts.Core.Input_System;
using Assets.Scripts.Core.InputSystem;
using Assets.Scripts.Entities;
using Assets.Scripts.Ui.Player;
using Photon.Pun;
using System;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    [RequireComponent(typeof(Entity))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerMovementLogic : MovementController
    {
        [Header("References")]
        [SerializeField] private Transform feets;
        [SerializeField] private Camera cam;

        [Header("Movement")]
        [SerializeField] private float landingVelocity;
        [SerializeField] private float keyInfluenceSpeed;
        [Min(0f)][SerializeField] private float feetLength;
        [Min(0f)][SerializeField] private float lockedMoveLerpSpeed;
        [Min(0f)][SerializeField] private float flyLerpSpeed;

        public event Action<float> OnLanded;

        public float CurrentVelocity { get; private set; }
        public bool isFlying { get; private set; }
        public bool IsGrounded { get; private set; }
        public bool IsWalkingAttemp { get; private set; }
        public bool IsWaking { get; private set; }
        public bool IsRunning { get; private set; }

        private PhotonView view => GetComponent<PhotonView>();
        private Rigidbody rb => GetComponent<Rigidbody>();
        private CapsuleCollider coll => GetComponent<CapsuleCollider>();
        private TouchscreenJoystick joystick => GetComponent<TouchscreenJoystick>();
        private Entity player => GetComponent<Entity>();

        private InputState inputSource;
        private float collSize;
        private float healthAspect;
        private float currentFlyLerpSpeed;
        private Vector3 keyInput;
        private Vector3 collCenter;
        private Vector3 moveVector;
        private Transform camTransform;

        public void Init(InputState inputSource)
        {
            this.inputSource = inputSource;

            collSize = coll.height;
            collCenter = coll.center;

            ResetMovement();

            currentFlyLerpSpeed = flyLerpSpeed;
            camTransform = cam.transform;

            inputSource.InputRecieved += Jump;
        }

        private void Update()
        {
            if (view.IsMine)
            {
                CurrentVelocity = rb.velocity.magnitude;

                keyInput = new Vector3(inputSource.VectorBinds[InputHandler.InputBind.WASD].x, 0f, inputSource.VectorBinds[InputHandler.InputBind.WASD].y);
                IsWaking = keyInput.sqrMagnitude > 0f && IsGrounded;
                IsWalkingAttemp = keyInput.sqrMagnitude > 0f;
                IsRunning = inputSource.BoolBinds[InputHandler.InputBind.RUNSTATE];

                if (isFlying)
                {
                    if (keyInput.sqrMagnitude > 0.1f)
                    {
                        ResetMovement();
                        InvokeMoveEvent();

                        Vector3 cameraForward = camTransform.forward;
                        Vector3 cameraRight = camTransform.right;

                        cameraForward = Vector3.Normalize(cameraForward);
                        cameraRight = Vector3.Normalize(cameraRight);

                        Vector3 moveDirection = cameraForward * keyInput.z + cameraRight * keyInput.x;

                        rb.velocity = Vector3.Lerp(rb.velocity, moveDirection * (IsRunning ? flySpeed * runSpeedBoost : flySpeed), currentFlyLerpSpeed * Time.deltaTime);
                    }
                    else
                        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, currentFlyLerpSpeed * Time.deltaTime);
                }
                else if (!isFlying && IsGrounded && keyInput.sqrMagnitude > 0f)
                {
                    ResetMovement();
                    InvokeMoveEvent();

                    moveVector = Vector3.Lerp(moveVector, transform.TransformDirection(keyInput) *
                        (inputSource.BoolBinds[InputHandler.InputBind.RUNSTATE] ? currentWalkSpeed * runSpeedBoost : currentWalkSpeed), keyInfluenceSpeed * Time.deltaTime);
                    moveVector.y = rb.velocity.y;

                    rb.velocity = moveVector;
                }

                healthAspect = Mathf.Max(player.Health / player.BaseHealth, 0.45f);
                currentWalkSpeed = data.WalkSpeed * healthAspect;
                flySpeed = data.FlySpeed * healthAspect;
                jumpPower = data.JumpPower * healthAspect;
                runSpeedBoost = Mathf.Max(data.RunSpeedBoost * healthAspect, 1f);
                currentFlyLerpSpeed = flyLerpSpeed * healthAspect;
            }
        }

        public void Jump(InputHandler.InputBind bind)
        {
            if (bind == InputHandler.InputBind.JUMP && (IsGrounded || isFlying))
                rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }

        private void OnCollisionEnter(Collision collision)
        {
            float contactSpeed = collision.relativeVelocity.magnitude;

            if (contactSpeed >= landingVelocity)
                OnLanded?.Invoke(contactSpeed);
        }
        private void FixedUpdate()
        {
            if (view.IsMine)
            {
                RaycastHit hit;
                IsGrounded = Physics.Raycast(feets.position, -feets.up, out hit, feetLength);
            }
        }

        private void ResetMovement()
        {
            currentWalkSpeed = data.WalkSpeed;
            runSpeedBoost = data.RunSpeedBoost;
            flySpeed = data.FlySpeed;
            jumpPower = data.JumpPower;
        }

        protected override void HandleFlyToggle(bool state)
        {
            rb.useGravity = !state;
            isFlying = state;
        }

        private void OnDestroy()
        {
            if (inputSource != null)
                inputSource.InputRecieved -= Jump;
        }
    }
}