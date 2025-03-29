using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Assets.Scripts.Spawning;

namespace Assets.Scripts.Network.Chat
{
    [RequireComponent(typeof(CommandParser))]
    [RequireComponent(typeof(Summonables))]
    public class ChatInputHandler : MonoBehaviourPunCallbacks
    {
        private const float COOLDOWN_BETWEEN_MESSAGE = 1.5f;

        [Header("Properties")]
        [SerializeField] private List<string> disallowedContent = new List<string>();

        public List<Message> Messages { get; private set; } = new List<Message>();

        public delegate void BeforeMessageSent(Message message, ref bool cancel);

        public event BeforeMessageSent OnBeforeMessageSent;
        public event Action<Message> OnMessageSent;
        public event Action<Message> OnMessageDeleted;

        private CommandParser commandParser => GetComponent<CommandParser>();
        private Summonables summoner => GetComponent<Summonables>();

        private EntityNetworkData playerData;
        private PhotonView playerView;
        private float cooldown;

        private void Update() => cooldown -= Time.deltaTime;

        public void Init(PhotonView playerView, EntityNetworkData playerData)
        {
            commandParser.OnSuccessfulParse += (log) => HandleMessage(new Message(Guid.NewGuid().ToString(), string.Empty, $"<b>{log}</b>", true), true);
            commandParser.OnFailureParse += (log) => HandleMessage(new Message(Guid.NewGuid().ToString(), string.Empty, $"<b><color=red>{log}</color></b>", true), true);

            this.playerView = playerView;
            this.playerData = playerData;

            if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
                return;

            HandleMessage(new Message(Guid.NewGuid().ToString(), string.Empty, $"<b><color=yellow>{PhotonNetwork.NickName} joined to the room</color></b>", ignoreFilter: true));
        }

        public void HandleMessage(Message msg, bool ignoreCommands = false)
        {
            if (playerView.IsMine)
            {
                bool cancel = false;

                OnBeforeMessageSent?.Invoke(msg, ref cancel);

                if (!string.IsNullOrEmpty(msg.Content) && !cancel)
                {
                    if ((!msg.Content.StartsWith(CommandParser.COMMANDS_PREFIX) || ignoreCommands) && (cooldown <= 0f || PhotonNetwork.IsMasterClient))
                    {
                        if (!msg.IgnoreFilter && !msg.LogMessage)
                        {
                            foreach (var content in disallowedContent)
                            {
                                msg.Content = msg.Content.Replace(content, "#");
                                msg.Author = msg.Author.Replace(content, "#");
                            }

                            msg.Author = $"{GetPrefix()} <color=#D1D1D1>{msg.Author}</color>";
                        }

                        cooldown = COOLDOWN_BETWEEN_MESSAGE;

                        Messages.Add(msg);
                        OnMessageSent?.Invoke(msg);
                    }
                    else if (!msg.LogMessage && !ignoreCommands)
                        if (!commandParser.TryParse(msg.Content, playerView.gameObject))
                            HandleMessage(msg, true);
                }
            }
        }
        public void DeleteMessage(string messageId)
        {
            Message msg = Messages.Find(n => n.Id == messageId);

            if (msg.Id != null)
            {
                Messages.Remove(msg);
                OnMessageDeleted?.Invoke(msg);
            }
        }

        private string GetPrefix()
        {
            string result = string.Empty;

            switch (playerData.EntityRights)
            {
                case EntityNetworkData.Rights.Moderator:
                    result = "<color=orange>[Moderator]</color>";
                    break;
                case EntityNetworkData.Rights.Absolute:
                    result = "<color=red>[Absolute]</color>";
                    break;
            }

            if (PhotonNetwork.IsMasterClient)
                result = "<color=blue>[Host]</color>";

            if (PlayerPrefs.GetInt(Connector.ADMIN_KEY) == 1)
                result = "<color=green>[Dev]</color>";

            return result;
        }
    }
}