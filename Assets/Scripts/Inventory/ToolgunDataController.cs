using Assets.Scripts.Core.Input_System;
using Assets.Scripts.Core.InputSystem;
using Assets.Scripts.Network;
using Assets.Scripts.Spawning;
using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Assets.Scripts.Inventory
{
    public class ToolgunDataController : ItemDataController
    {
        [Header("References")]
        [SerializeField] private Transform holdPoint;
        [SerializeField] private Transform scanPos;

        [Header("Properties")]
        [SerializeField] private LayerMask holdableLayer;
        [SerializeField] private float holdedObjVelocityStabilizer;
        [SerializeField] private float holdedObjFollowSpeed;
        [SerializeField] private float holdPointScrollSpeed;
        [SerializeField] private float scanDistance;

        private InputMachine inputMachine;
        private InputState inputSource;
        private PhotonRigidbodyView rbView;
        private Summonables summoner;
        private Rigidbody holdedObjectRb;
        private Transform holdedObject;
        private bool holdedObjectGravityUse;
        private bool initialized;
        private bool privateMode;

        public int selectedObjectId = -1;

        public void Init(Summonables summoner, InputMachine inputMachine, InputState inputSource)
        {
            this.inputSource = inputSource;
            this.inputMachine = inputMachine;
            this.summoner = summoner;

            initialized = true;
            Debug.Log(Convert.ToBoolean(PhotonNetwork.CurrentRoom.CustomProperties[Connector.ROOM_HASHTABLE_PRIVATE_KEY]));
            privateMode = Convert.ToBoolean(PhotonNetwork.CurrentRoom.CustomProperties[Connector.ROOM_HASHTABLE_PRIVATE_KEY]);
        }

        private void Update()
        {
            if (!initialized)
                return;

            if (inputSource.VectorBinds[InputHandler.InputBind.MOUSEWHEEL].y != 0f && holdedObject != null)
            {
                holdPoint.localPosition = new Vector3(0f, 0f, holdPoint.localPosition.z + inputSource.VectorBinds[InputHandler.InputBind.MOUSEWHEEL].y * holdPointScrollSpeed * Time.deltaTime);
            }

            if (inputSource.BoolBinds[InputHandler.InputBind.MOUSELEFT] && holdedObject != null && (holdedObjectRb != null || rbView != null))
            {
                Vector3 targetPos = Vector3.Lerp(holdedObject.position, holdPoint.position, holdedObjFollowSpeed * Time.deltaTime);

                if (rbView == null)
                    holdedObject.position = targetPos;
                else
                    rbView.photonView.RPC(nameof(PhotonRigidbodyView.SetPosition), RpcTarget.All, targetPos);
            }
            else
            {
                if (holdedObjectRb != null || rbView != null)
                {
                    Vector3 targetVelocity = (holdPoint.position - holdedObject.position) * holdedObjVelocityStabilizer * Time.deltaTime;

                    if (rbView == null)
                    {
                        holdedObjectRb.useGravity = holdedObjectGravityUse;
                        holdedObjectRb.velocity = targetVelocity;
                    }
                    else
                    {
                        rbView.photonView.RPC(nameof(PhotonRigidbodyView.SetGravityUse), RpcTarget.All, holdedObjectGravityUse);
                        rbView.photonView.RPC(nameof(PhotonRigidbodyView.SetVelocity), RpcTarget.All, targetVelocity);
                    }
                }

                rbView = null;
                holdedObjectRb = null;
                holdedObject = null;

                inputMachine.SetBindActiveForEveryState(InputHandler.InputBind.MOUSEWHEEL, true);
                OnEndActionEventTrigger();
            }
        }

        private void OnBindToggle(InputHandler.InputBind bind)
        {
            if (bind == InputHandler.InputBind.MOUSELEFT)
            {
                RaycastHit hit;

                if (Physics.Raycast(scanPos.position, scanPos.forward, out hit, scanDistance, holdableLayer, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider.TryGetComponent<PhotonRigidbodyView>(out var rbV))
                    {
                        inputMachine.SwitchBindState(InputHandler.InputBind.MOUSEWHEEL, inputMachine.GetStates.IndexOf(inputSource));

                        holdedObject = hit.collider.transform;
                        rbView = rbV;

                        holdedObjectGravityUse = rbView.m_Body.useGravity;
                        holdPoint.position = hit.point;

                        rbView.photonView.RPC(nameof(PhotonRigidbodyView.SetGravityUse), RpcTarget.All, false);
                        OnStartActionEventTrigger();
                    }
                    else if (hit.collider.TryGetComponent<Rigidbody>(out var rb))
                    {
                        inputMachine.SwitchBindState(InputHandler.InputBind.MOUSEWHEEL, inputMachine.GetStates.IndexOf(inputSource));

                        holdedObject = hit.collider.transform;
                        holdedObjectRb = rb;

                        holdedObjectGravityUse = rb.useGravity;
                        holdPoint.position = hit.point;

                        rb.useGravity = false;

                        OnStartActionEventTrigger();
                    }
                }
            }
            else if (bind == InputHandler.InputBind.MOUSERIGHT)
            {
                RaycastHit hit;

                if (Physics.Raycast(scanPos.position, scanPos.forward, out hit, scanDistance, holdableLayer, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider.TryGetComponent<PhotonView>(out var collView))
                        PhotonNetwork.Destroy(collView);
                }
            }
            else if (bind == InputHandler.InputBind.MOUSEMID && selectedObjectId != -1)
            {
                RaycastHit hit;

                if (Physics.Raycast(scanPos.position, scanPos.forward, out hit, scanDistance, ~0, QueryTriggerInteraction.Ignore))
                    summoner.Summon(selectedObjectId, hit.point, Vector3.one, Quaternion.LookRotation(Vector3.ProjectOnPlane(Vector3.forward, hit.normal), hit.normal), null, privateMode);
            }
        }

        private void OnDisable() => inputSource.InputRecieved -= OnBindToggle;
        private void OnEnable() => inputSource.InputRecieved += OnBindToggle;
    }
}