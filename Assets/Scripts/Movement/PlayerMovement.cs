using Assets.Scripts.Core.Input_System;
using Assets.Scripts.Core.InputSystem;
using Assets.Scripts.Entities;
using Assets.Scripts.Ui.Player;
using Photon.Pun;
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
        [SerializeField] private float keyInfluenceSpeed;
        [Min(0f)][SerializeField] private float feetLength;
        [Min(0f)][SerializeField] private float lockedMoveLerpSpeed;
        [Min(0f)][SerializeField] private float flyLerpSpeed;

        public float CurrentVelocity { get; private set; }
        public bool isFlying { get; private set; }
        public bool IsGrounded { get; private set; }
        public bool IsWaking { get; private set; }

        public InputState inputSource;

        private PhotonView view => GetComponent<PhotonView>();
        private Rigidbody rb => GetComponent<Rigidbody>();
        private CapsuleCollider coll => GetComponent<CapsuleCollider>();
        private TouchscreenJoystick joystick => GetComponent<TouchscreenJoystick>();
        private Entity player => GetComponent<Entity>();

        private float collSize;
        private float healthAspect;
        private float currentFlyLerpSpeed;
        private Vector3 keyInput;
        private Vector3 collCenter;
        private Vector3 moveVector;
        private Transform camTransform;

        public void Init()
        {
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
                keyInput = new Vector3(inputSource.VectorBinds[InputHandler.InputBind.WASD].x, 0f, inputSource.VectorBinds[InputHandler.InputBind.WASD].y);
                IsWaking = keyInput.sqrMagnitude > 0f && IsGrounded;

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

                        rb.velocity = Vector3.Lerp(rb.velocity, moveDirection * (inputSource.BoolBinds[InputHandler.InputBind.RUNSTATE]() ? flySpeed * runSpeedBoost : flySpeed), currentFlyLerpSpeed * Time.deltaTime);
                    }
                    else
                        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, currentFlyLerpSpeed * Time.deltaTime);
                }
                else if (!isFlying && IsGrounded && keyInput.sqrMagnitude > 0f)
                {
                    ResetMovement();
                    InvokeMoveEvent();

                    moveVector = Vector3.Lerp(moveVector, transform.TransformDirection(keyInput) *
                        (inputSource.BoolBinds[InputHandler.InputBind.RUNSTATE]() ? currentWalkSpeed * runSpeedBoost : currentWalkSpeed), keyInfluenceSpeed * Time.deltaTime);
                    moveVector.y = rb.velocity.y;

                    rb.velocity = moveVector;
                }

                CurrentVelocity = rb.velocity.magnitude;

                healthAspect = Mathf.Max(player.Health / player.BaseHealth, 0.45f);
                currentWalkSpeed = data.WalkSpeed * healthAspect;
                flySpeed = data.FlySpeed * healthAspect;
                jumpPower = data.JumpPower * healthAspect;
                runSpeedBoost = Mathf.Max(data.RunSpeedBoost * healthAspect, 1f);
                currentFlyLerpSpeed = flyLerpSpeed * healthAspect;
            }
            if (view.IsMine)
            {
                if (isFlying)
                    rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, currentFlyLerpSpeed * Time.deltaTime);
                else if (!isFlying && IsGrounded)
                {
                    Vector3 targetVel = rb.velocity;
                    targetVel.x = Mathf.Lerp(targetVel.x, 0f, currentWalkSpeed * Time.deltaTime);
                    targetVel.z = Mathf.Lerp(targetVel.z, 0f, currentWalkSpeed * Time.deltaTime);

                    rb.velocity = targetVel;
                }
            }
        }
        public void Jump(InputHandler.InputBind bind)
        {
            if (bind == InputHandler.InputBind.JUMP && (IsGrounded || isFlying))
                rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
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
    }
}