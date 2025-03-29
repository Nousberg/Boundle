using Photon.Pun;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class PlayerNetworkManager : MonoBehaviour
    {
        public PhotonView view => GetComponent<PhotonView>();

        [PunRPC]
        public void Kick(string reason = "not specified")
        {
            if (view.IsMine)
            {
                PhotonNetwork.LeaveRoom();

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
