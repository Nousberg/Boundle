using UnityEngine;
using Photon.Pun;

namespace Assets.Scripts.Core.Environment
{
    public class SceneData : MonoBehaviour
    {
        public Vector3 PlayersSpawnPosition { get; private set; } = new Vector3(-110.76f, 31.89f, -29.96f);
        public Quaternion PlayersSpawnRotation { get; private set; } = new Quaternion(0f, -60.122f, 0f, 0f);

        [PunRPC]
        public void SetPlayersSpawnPoint(Vector3 spawnPosition, Quaternion spawnRotation)
        {
            if (!(float.IsNaN(spawnPosition.x) && float.IsNaN(spawnPosition.y) && float.IsNaN(spawnPosition.z)))
                PlayersSpawnPosition = spawnPosition;
            if (!(float.IsNaN(spawnRotation.x) && float.IsNaN(spawnRotation.y) && float.IsNaN(spawnRotation.z)))
                PlayersSpawnRotation = spawnRotation;
        }
    }
}
