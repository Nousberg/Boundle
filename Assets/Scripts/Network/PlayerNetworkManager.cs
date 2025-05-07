using Assets.Scripts.Core;
using Photon.Pun;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class PlayerNetworkManager : MonoBehaviour
    {
        public PhotonView view => GetComponent<PhotonView>();

        [PunRPC]
        public void Kick(string reason = "not specified", bool isBan = false, bool native = false)
        {
            if (view.IsMine)
            {
                PhotonNetwork.LeaveRoom();

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                if (!native)
                {
                    DataContainer.wasBanned = isBan;
                    DataContainer.wasKicked = !isBan;
                }
            }
        }
    }
}
