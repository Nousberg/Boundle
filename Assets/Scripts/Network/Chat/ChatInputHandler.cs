using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Network.Chat
{
    [RequireComponent(typeof(CommandParser))]
    public class ChatInputHandler : MonoBehaviour
    {
        private const float COOLDOWN_BETWEEN_MESSAGE = 0f;

        [Header("Properties")]
        [SerializeField] private List<string> disallowedContent = new List<string>();

        public List<Message> Messages { get; private set; } = new List<Message>();

        public delegate void BeforeMessageSent(Message message, ref bool cancel);

        public event BeforeMessageSent OnBeforeMessageSent;
        public event Action<Message> OnMessageSent;
        public event Action<Message> OnMessageDeleted;

        private CommandParser commandParser => GetComponent<CommandParser>();
        private GameObject currentMessage;
        private float cooldown;

        private void Update() => cooldown -= Time.deltaTime;

        public void Init()
        {
            commandParser.OnSuccessfulParse += (log) => HandleMessage(new Message(Guid.NewGuid(), string.Empty, log, logMessage: true));
            commandParser.OnFailureParse += (log) => HandleMessage(new Message(Guid.NewGuid(), string.Empty, log, logMessage: true));
        }
        public void HandleMessage(Message msg, GameObject sender = null, bool ignoreCommands = false)
        {
            bool cancel = false;
            
            OnBeforeMessageSent?.Invoke(msg, ref cancel);

            if (!string.IsNullOrEmpty(msg.Content) && !cancel)
            {
                if (!msg.Content.StartsWith(CommandParser.COMMANDS_PREFIX) && cooldown <= 0f)
                {
                    string filteredContent = msg.Content;

                    foreach (var content in disallowedContent)
                        filteredContent = filteredContent.Replace(content, "#");

                    cooldown = COOLDOWN_BETWEEN_MESSAGE;
                    OnMessageSent?.Invoke(new Message(msg.Id, msg.Author, msg.Content, msg.ContentColor));
                }
                else if (!ignoreCommands && sender != null)
                    if (!commandParser.TryParse(msg.Content, sender))
                        HandleMessage(msg, sender, ignoreCommands: true);
            }
        }
        public void DeleteMessage(Guid messageId)
        {
            Message msg = Messages.Find(n => n.Id == messageId);

            if (msg.Id != null)
            {
                Messages.Remove(msg);
                OnMessageDeleted?.Invoke(msg);
            }
        }
    }
}