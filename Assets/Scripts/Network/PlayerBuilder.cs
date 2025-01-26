using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class PlayerBuilder : MonoBehaviour
    {
        [SerializeField] private TMP_InputField playerName;

        private void Start() => playerName.text = PlayerPrefs.GetString("playerName");

        public void SetNickname(string value)
        {
            PhotonNetwork.NickName = value;
            PlayerPrefs.SetString("playerName", value);
        }
    }
}
