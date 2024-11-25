using Assets.Scripts.Entities;
using Assets.Scripts.Ui.Chat;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Network
{
    [RequireComponent(typeof(CommandParser))]
    public class ChatInputHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform messagesListBox;
        [SerializeField] private GameObject messagePrefab;

        [Header("Properties")]
        [SerializeField] private List<string> disallowedContent = new List<string>();

        private CommandParser commandParser => GetComponent<CommandParser>();
        private GameObject currentMessage;

        private void Start()
        {
            commandParser.OnSuccefulParse += HandleParserLog;
            commandParser.OnFailureParse += HandleParserLog;
        }

        public void HandleMessage(string text, string senderName, Entity sender = null)
        {
            if (!string.IsNullOrEmpty(text) && senderName != null)
                if (text[0] != '/')
                {
                    foreach (var content in disallowedContent)
                        text = text.Replace(content, "#");

                    if (currentMessage != null)
                        Destroy(currentMessage);

                    currentMessage = Instantiate(messagePrefab, messagesListBox);
                    Message message = currentMessage.GetComponent<Message>();

                    if (message != null)
                    {
                        message.MessageText.text = text;
                        message.AuthorText.text = senderName + (senderName != string.Empty ? ": " : string.Empty);
                    }
                }
                else if (sender != null)
                    commandParser.TryParse(text, sender);
        }
        public void HandleParserLog(string log) => HandleMessage(log, string.Empty);
    }
}