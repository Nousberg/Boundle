using Assets.Scripts.Spawning;
using UnityEngine;

namespace Assets.Scripts.Inventory
{
    public class ToolgunDataController : ItemDataController
    {
        [Header("References")]
        [SerializeField] private Summonables summoner;
        [SerializeField] private Transform scanPos;

        [Header("Properties")]
        [SerializeField] private float scanDistance;

        private int selectedObjectId;

        public void SelectObjectToSpawn(int id) => selectedObjectId = id;

        private void Update()
        {
            if (Input.GetMouseButtonDown(2))
            {
                RaycastHit hit;

                if (Physics.Raycast(scanPos.position, scanPos.forward, out hit, scanDistance))
                {
                    summoner.Summon(selectedObjectId, hit.point, Quaternion.LookRotation(Vector3.forward, -hit.normal), null);
                }
            }
        }
    }
}