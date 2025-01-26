using UnityEngine;
using Photon.Pun;

namespace Assets.Scripts.Core.Environment
{
    public class SceneData : MonoBehaviour
    {
        public Vector3 PlayersSpawnPosition { get; private set; } = new Vector3(0, 35, 0);
        public Quaternion PlayersSpawnRotation { get; private set; }

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
