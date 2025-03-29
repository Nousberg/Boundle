using Cysharp.Threading.Tasks;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Saving;

namespace Assets.Scripts.Network
{
    [RequireComponent(typeof(PhotonView))]
    public class PacketSender : MonoBehaviour
    {
        private const int PACKET_SEND_INTERVAL = 2000;

        public List<PacketOverwrite> Overwrites { get; private set; } = new List<PacketOverwrite>();
        public List<string> EntityWrites { get; private set; } = new List<string>();
        public List<string> ObjectWrites { get; private set; } = new List<string>();
        public List<string> DroppedItemWrites { get; private set; } = new List<string>();

        private PhotonView view => GetComponent<PhotonView>();

        public class PacketOverwrite
        {
            public int networkId;
            public string jsonData;
        }
    }
}