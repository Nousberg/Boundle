using Assets.Scripts.Entities;
using Photon.Pun;
using ExitGames.Client.Photon;

using Photon.Realtime;
using UnityEngine;
using Assets.Scripts.Core.Interfaces;

namespace Assets.Scripts.Network
{
    [RequireComponent(typeof(Entity))]
    [RequireComponent(typeof(PhotonView))]
    public class LocalEntity : MonoBehaviourPunCallbacks, IDamageable
    {
        public float Health { get; private set; }

        private Entity remoteEntity => GetComponent<Entity>();
        private PhotonView view => GetComponent<PhotonView>();

        private void Start()
        {
            remoteEntity.OnHealthChanged += CallRpc;
        }

        [PunRPC]
        public void TakeDamage(float amount, LocalEntity attacker, DamageType type)
        {
            if (view.IsMine)
                remoteEntity.TakeDamage(amount, attacker, type);
        }

        private void CallRpc() => view.RPC("SyncHealth", RpcTarget.Others);

        [PunRPC]
        private void SyncHealth()
        {
            Health = remoteEntity.Health;
        }
    }
}