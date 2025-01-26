using Assets.Scripts.Network.Chat;
using Photon.Pun;
using System;
using UnityEngine;

namespace Assets.Scripts.Ui.Chat
{
    public class ChatInteractProvider : MonoBehaviour
    {
        private ChatInputHandler chat;

        public void Init(ChatInputHandler cHandler)
        {
            chat = cHandler;
            Send("/effect add self godness 1 50 true");
        }

        public void Send(string message) => chat.HandleMessage(new Message(Guid.NewGuid(), PhotonNetwork.NickName, message), gameObject);
    }
}
