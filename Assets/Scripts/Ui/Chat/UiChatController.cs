using Assets.Scripts.Network.Chat;
using Assets.Scripts.Network;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Ui.Chat
{
    [RequireComponent(typeof(PhotonView))]
    public class UiChatController : MonoBehaviour
    {
        [SerializeField] private Transform messageList;
        [SerializeField] private Transform globalMessageList;
        [SerializeField] private GameObject messagePrefab;
        private PhotonView view => GetComponent<PhotonView>();

        private ChatInputHandler chat;
        private CommandParser commandParser;

        private List<MessageUiData> messages = new List<MessageUiData>();
        private List<MessageUiData> globalMessages = new List<MessageUiData>();

        public void Init(ChatInputHandler c, CommandParser p)
        {
            chat = c;
            commandParser = p;

            chat.OnMessageSent += (message) => {
                if (!message.LogMessage)
                    view.RPC(nameof(ShowMessageNative), RpcTarget.All, JsonUtility.ToJson(message));
                else
                    ShowMessageNative(JsonUtility.ToJson(message));
            };
            chat.OnMessageDeleted += (message) => {
                if (!message.LogMessage)
                    view.RPC(nameof(DeleteMessage), RpcTarget.All, message.Id); 
            };
        }

        [PunRPC]
        public void ShowMessageNative(string jsonData)
        {
            Message data = JsonUtility.FromJson<Message>(jsonData);

            ShowMessage(data, messageList, true);
            ShowMessage(data, globalMessageList, false);
        }
        [PunRPC]
        private void DeleteMessage(string guid)
        {
            MessageUiData msg = messages.Find(n => n.Id == guid);

            if (msg != null)
            {
                messages.Remove(msg);
                Destroy(msg.gameObject);
            }

            MessageUiData globalMsg = globalMessages.Find(n => n.Id == guid);

            if (globalMsg != null)
            {
                globalMessages.Remove(globalMsg);
                Destroy(globalMsg.gameObject);
            }
        }
        private void ShowMessage(Message data, Transform parent, bool animated)
        {
            GameObject messageObj = Instantiate(messagePrefab, parent);

            if (messageObj.TryGetComponent<MessageUiData>(out var msg))
            {
                msg.ContentText.text = (string.IsNullOrEmpty(data.Author) ? string.Empty : data.Author + " : ") + data.Content;
                msg.Init(data.Id, animated);

                msg.OnDestroyEvent += () => messages.Remove(msg);

                if (!data.LogMessage)
                {
                    messages.Add(msg);
                    globalMessages.Add(msg);
                }
            }
        }
    }
}
