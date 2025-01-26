using Assets.Scripts.Spawning;
using Photon.Pun;
using System;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class EntityNetworkData : MonoBehaviour
    {
        [field: SerializeField] public Rights EntityRights { get; private set; }
        [SerializeField] private bool immutableNametag;

        [field: SerializeField] public string Nametag { get; private set; }
        public event Action OnNametagEdit;

        public PhotonView view => GetComponent<PhotonView>();

        private void Start()
        {
            if (GetComponent<SummonablePlayer>() != null)
                immutableNametag = true;
        }
        [PunRPC]
        public void SetRigths(EntityNetworkData initiator, Rights rigths, bool ignoreDifference)
        {
            if ((initiator.EntityRights < EntityRights && rigths > initiator.EntityRights) || ignoreDifference)
                EntityRights = rigths;
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

        public enum Rights : byte
        {
            None,
            Default,
            Moderator,
            Absolute
        }
    }
}
