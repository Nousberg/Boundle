using UnityEngine;
using Photon.Pun;
using Assets.Scripts.Inventory;

namespace Assets.Scripts.PunUtils
{
    [RequireComponent(typeof(DroppedItem))]
    public class PhotonItemDataView : MonoBehaviourPun, IPunObservable
    {
        private DroppedItem item => GetComponent<DroppedItem>();

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(item.data);
            }
            else
            {
                item.data = (string)stream.ReceiveNext();
            }
        }
    }
}