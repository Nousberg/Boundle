using UnityEngine;
using Photon.Pun;
using TMPro;

namespace Assets.Scripts.Ui.Multiplayer
{
    public class PlayerInfoVisualizer : MonoBehaviourPun, IPunObservable
    {
        [SerializeField] private Camera cm;
        [SerializeField] private TextMeshProUGUI nonLocalPlayerName;
        [SerializeField] private Transform nonLocalPlayerInfoUi;

        private Transform lookTarget;

        private void Update()
        {
            if (!photonView.IsMine)
            {
                if (lookTarget == null)
                {
                    Camera[] cams = FindObjectsOfType<Camera>();

                    foreach (Camera c in cams)
                        if (cm.GetInstanceID() != c.GetInstanceID() && c.gameObject.activeInHierarchy)
                        {
                            lookTarget = c.transform;
                            break;
                        }
                }

                nonLocalPlayerInfoUi.LookAt(lookTarget);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(PhotonNetwork.NickName);
            }
            else
            {
                nonLocalPlayerName.text = (string)stream.ReceiveNext();
            }
        }
    }
}
