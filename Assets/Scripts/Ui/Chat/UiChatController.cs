using Assets.Scripts.Network.Chat;
using Assets.Scripts.Network;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Ui.Chat
{
    [RequireComponent(typeof(PhotonView))]
    public class UiChatController : MonoBehaviour
    {
        [SerializeField] private Transform messageList;
        [SerializeField] private GameObject messagePrefab;

        private PhotonView view => GetComponent<PhotonView>();

        private ChatInputHandler chat;
        private CommandParser commandParser;

        private List<MessageUiData> messages = new List<MessageUiData>();

        public void Init(ChatInputHandler c, CommandParser p)
        {
            chat = c;
            commandParser = p;

            chat.OnMessageSent += (message) => {
                if (!message.LogMessage)
                    view.RPC("ShowMessage", RpcTarget.All, JsonUtility.ToJson(message));
                else
                    ShowMessage(JsonUtility.ToJson(message));
            };
            chat.OnMessageDeleted += (message) => {
                if (!message.LogMessage)
                    view.RPC("DeleteMessage", RpcTarget.All, message.Id); 
            };
        }

        [PunRPC]
        private void ShowMessage(string jsonData)
        {
            Message data = JsonUtility.FromJson<Message>(jsonData);

            GameObject messageObj = Instantiate(messagePrefab, messageList);

            if (messageObj.TryGetComponent<MessageUiData>(out var msg))
            {
                if (data.ContentColor.IsAssigned())
                    msg.ContentText.color = data.ContentColor.ToUnityColor();

                msg.ContentText.text = (string.IsNullOrEmpty(data.Author) ? string.Empty : data.Author + ": ") + data.Content;
                msg.Init(data.Id);

                msg.OnDestroyEvent += HandleMessageAnimationEnd;
            }
        }
        [PunRPC]
        private void DeleteMessage(string guid)
        {
            Guid id = Guid.Parse(guid);

            MessageUiData msg = messages.Find(n => n.Id == id);

            if (msg != null)
            {
                messages.Remove(msg);
                Destroy(msg);
            }
        }
        private void HandleMessageAnimationEnd(MessageUiData msg) => messages.Remove(msg);
    }
}
