using Photon.Pun;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class PlayerNetworkManager : MonoBehaviour
    {
        public PhotonView view => GetComponent<PhotonView>();

        [PunRPC]
        public void Kick()
        {
            if (view.IsMine)
                PhotonNetwork.LeaveRoom();
        }
    }
}
