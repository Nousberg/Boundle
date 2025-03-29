using Assets.Scripts.Spawning;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class EntityNetworkData : MonoBehaviourPunCallbacks, IPunObservable
    {
        [field: SerializeField] public Rights EntityRights { get; private set; }
        [SerializeField] private bool immutableNametag;

        [field: SerializeField] public string Nametag { get; private set; }
        [field: SerializeField] public string UUID { get; private set; }

        public event Action OnNametagEdit;
        public event Action OnRightsEdit;

        public PhotonView view => GetComponent<PhotonView>();

        private void Start()
        {
            if (GetComponent<SummonablePlayer>() != null)
                immutableNametag = true;
        }
        [PunRPC]
        private void RPC_SetRights(byte targetRights)
        {
            EntityRights = (Rights)targetRights;
            OnRightsEdit?.Invoke();
        }

        public bool SetRigths(EntityNetworkData initiator, Rights rigths, bool ignoreDifference)
        {
            bool result = (initiator.EntityRights > EntityRights && rigths > initiator.EntityRights) || ignoreDifference;

            if (result)
                view.RPC(nameof(RPC_SetRights), view.Owner, (byte)rigths);

            return result;
        }
        [PunRPC]
        public void SetNametag(string value)
        {
            if (!immutableNametag && !string.IsNullOrEmpty(value))
            {
                Nametag = value;
                OnNametagEdit?.Invoke();
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext((byte)EntityRights);
                stream.SendNext(Nametag);
            }
            else
            {
                EntityRights = (Rights)stream.ReceiveNext();
                Nametag = (string)stream.ReceiveNext();
            }
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (photonView.IsMine)
            {
                OnRightsEdit.Invoke();
                if (EntityRights == Rights.Host && newMasterClient != PhotonNetwork.LocalPlayer)
                    SetRigths(this, Rights.Default, true);
            }
        }

        public enum Rights : byte
        {
            None,
            Default,
            Moderator,
            Absolute,
            Host
        }
    }
}
