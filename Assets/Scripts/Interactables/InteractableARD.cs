using System;
using Assets.Scripts.Network;
using Assets.Scripts.Ui.ARD;
using Photon.Pun;
using UnityEngine;

namespace Assets.Scripts.Interactables
{
    public class InteractableARD : MonoBehaviour, Interactable, IPunObservable
    {
        [SerializeField] private GameObject rocketPrefab;
        [SerializeField] private int rocketAmount;
        [SerializeField] private float fireRate;
        [SerializeField] private float reloadDuration;

        private InterfaceARD ui;
        private int overallAmmo;
        private bool privateServer;

        public void Init(InterfaceARD ui)
        {
            this.ui = ui;
            privateServer = Convert.ToBoolean(PhotonNetwork.CurrentRoom.CustomProperties[Connector.ROOM_HASHTABLE_PRIVATE_KEY]);
        }

        public void EnableInteract()
        {

        }
        public void DisableInteract()
        {

        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (privateServer)
                return;

            if (stream.IsWriting)
            {
                stream.SendNext(overallAmmo);
            }
            else
            {
                overallAmmo = (int)stream.ReceiveNext();
            }
        }

        private enum Target : byte
        {
            Player,
            Bot,
            Vehicle
        }
    }
}
