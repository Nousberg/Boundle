using Assets.Scripts.Network.Chat;
using Photon.Pun;
using System;
using UnityEngine;

namespace Assets.Scripts.Ui.Chat
{
    [RequireComponent(typeof(ChatInputHandler))]
    public class ChatInteractProvider : MonoBehaviour
    {
        private ChatInputHandler chatHandler => GetComponent<ChatInputHandler>();

        public void Send(string message) => chatHandler.HandleMessage(new Message(Guid.NewGuid().ToString(), PhotonNetwork.NickName, message));
    }
}
