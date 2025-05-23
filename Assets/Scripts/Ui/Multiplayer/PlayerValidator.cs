﻿using Assets.Scripts.Network;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Ui.Multiplayer
{
    public class PlayerValidator : MonoBehaviourPunCallbacks
    {
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public void Init(EntityNetworkData entityData)
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("playerName");

            if (string.IsNullOrEmpty(PhotonNetwork.NickName))
            {
                PhotonNetwork.NickName = "Player" + Random.Range(0, int.MaxValue / short.MaxValue);
                PlayerPrefs.SetString("playerName", PhotonNetwork.NickName);
            }
            entityData.view.RPC("SetNametag", RpcTarget.All, PhotonNetwork.NickName);
        }
    }
}
