using Assets.Scripts.Spawning;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Inventory
{
    public class ToolgunDataController : ItemDataController
    {
        [Header("References")]
        [SerializeField] private Summonables summoner;
        [SerializeField] private Transform holdPoint;
        [SerializeField] private Transform scanPos;

        [Header("Properties")]
        [SerializeField] private LayerMask holdableLayer;
        [SerializeField] private float holdedObjFollowSpeed;
        [SerializeField] private float holdPointScrollSpeed;
        [SerializeField] private float scanDistance;

        public static bool IsHolding { get; private set; }

        private int selectedObjectId;
        private bool holdedObjectGravityUse;
        private Vector3 initHoldPoint;
        private Rigidbody holdedObjectRb;
        private Transform holdedObject;

        public void SelectObjectToSpawn(int id) => selectedObjectId = id;

        private void Start()
        {
            initHoldPoint = holdPoint.localPosition;
        }

        private void Update()
        {
            if (Input.mouseScrollDelta.y != 0f && holdedObject != null && holdedObjectRb != null)
            {
                holdPoint.localPosition = new Vector3(0f, 0f, holdPoint.localPosition.z + Input.mouseScrollDelta.y * holdPointScrollSpeed);
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
                    if (hit.collider.TryGetComponent<Rigidbody>(out var rb))
                    {
                        IsHolding = true;
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
                    Destroy(hit.collider.gameObject);
            }
            else if (Input.GetMouseButton(0) && holdedObject != null && holdedObjectRb != null)
            {
                float pointMg = holdPoint.position.magnitude;
                float objMg = holdedObject.position.magnitude;

                holdedObjectRb.velocity = (holdPoint.position - holdedObject.position).normalized * Mathf.Lerp(0f, holdedObjFollowSpeed, 1f - (Mathf.Min(objMg, pointMg) / Mathf.Max(objMg, pointMg))) * Time.deltaTime;
            }
            else
            {
                if (holdedObjectRb != null)
                    holdedObjectRb.useGravity = holdedObjectGravityUse;

                holdedObjectRb = null;
                holdedObject = null;

                holdPoint.localPosition = initHoldPoint;

                IsHolding = false;
            }
        }
    }
}