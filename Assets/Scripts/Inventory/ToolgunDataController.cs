using Assets.Scripts.Core.Input_System;
using Assets.Scripts.Spawning;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

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

        public InputState inputSource;

        private InputMachine inputMachine;
        private PhotonRigidbodyView rbView;
        private Summonables summoner;
        private int selectedObjectId = 4;
        private bool holdedObjectGravityUse;
        private Rigidbody holdedObjectRb;
        private Transform holdedObject;

        public void SelectObjectToSpawn(int id) => selectedObjectId = id;

        public void Init(Summonables summoner, InputMachine inputMachine)
        {
            this.inputMachine = inputMachine;
            this.summoner = summoner;
        }

        private void Update()
        {
            if (inputSource.VectorBinds[Core.InputSystem.InputHandler.InputBind.MOUSEWHEEL].y != 0f && holdedObject != null && holdedObjectRb != null)
            {
                holdPoint.localPosition = new Vector3(0f, 0f, holdPoint.localPosition.z + inputSource.VectorBinds[Core.InputSystem.InputHandler.InputBind.MOUSEWHEEL].y * holdPointScrollSpeed);
            }
            if (Input.GetMouseButtonDown(2))
            {
                RaycastHit hit;

                if (Physics.Raycast(scanPos.position, scanPos.forward, out hit, scanDistance))
                {
                    summoner.Summon(selectedObjectId, hit.point, Quaternion.LookRotation(Vector3.forward, -hit.normal), null);
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;

                if (Physics.Raycast(scanPos.position, scanPos.forward, out hit, scanDistance, holdableLayer, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider.TryGetComponent<PhotonRigidbodyView>(out var rbV))
                    {
                        inputMachine.SwitchBindState(Core.InputSystem.InputHandler.InputBind.MOUSEWHEEL, inputMachine.GetStates.IndexOf(inputSource));

                        holdedObject = hit.collider.transform;
                        rbView = rbV;

                        holdedObjectGravityUse = rbView.m_Body.useGravity;
                        holdPoint.position = hit.point;

                        rbView.photonView.RPC("SetGravityUse", RpcTarget.All, false);
                    }
                    else if (hit.collider.TryGetComponent<Rigidbody>(out var rb))
                    {
                        inputMachine.SwitchBindState(Core.InputSystem.InputHandler.InputBind.MOUSEWHEEL, inputMachine.GetStates.IndexOf(inputSource));

                        holdedObject = hit.collider.transform;
                        holdedObjectRb = rb;

                        holdedObjectGravityUse = rb.useGravity;
                        holdPoint.position = hit.point;

                        rb.useGravity = false;
                    }
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                RaycastHit hit;

                if (Physics.Raycast(scanPos.position, scanPos.forward, out hit, scanDistance, holdableLayer, QueryTriggerInteraction.Ignore))
                    PhotonNetwork.Destroy(hit.collider.gameObject);
            }
            else if (Input.GetMouseButton(0) && holdedObject != null && (holdedObjectRb != null || rbView != null))
            {
                if (rbView == null)
                    holdedObject.position = Vector3.Lerp(holdedObject.position, holdPoint.position, holdedObjFollowSpeed * Time.deltaTime);
                else
                    rbView.photonView.RPC("SetVelocity", RpcTarget.All, (holdPoint.position - holdedObject.position) * holdedObjVelocityStabilizer * Time.deltaTime);
            }
            else
            {
                if (holdedObjectRb != null || rbView != null)
                {
                    if (rbView == null)
                    {
                        holdedObjectRb.useGravity = holdedObjectGravityUse;
                        holdedObjectRb.velocity = (holdPoint.position - holdedObject.position) * holdedObjVelocityStabilizer * Time.deltaTime;
                    }
                    else
                    {
                        rbView.photonView.RPC("SetGravityUse", RpcTarget.All, holdedObjectGravityUse);
                    }
                }

                rbView = null;
                holdedObjectRb = null;
                holdedObject = null;

                inputMachine.SetBindActiveForEveryState(Core.InputSystem.InputHandler.InputBind.MOUSEWHEEL, false);
            }
        }
    }
}