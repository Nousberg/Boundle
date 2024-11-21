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

        public void HandleMessage(string text, string senderName, Entity sender = null)
        {
            if (!string.IsNullOrEmpty(text) && senderName != null)
                if (text[0] != '/')
                {
                    foreach (var content in disallowedContent)
                        if (text.Contains(content))
                        {
                            int startIndex = text.IndexOf(content[0]);
                            int endIndex = text.IndexOf(content[content.Length - 1]) - startIndex;

                            text.Remove(startIndex, endIndex);
                        }

                    if (currentMessage != null)
                        Destroy(currentMessage);

                    currentMessage = Instantiate(messagePrefab, messagesListBox);
                    Message message = currentMessage.GetComponent<Message>();

                    if (message != null)
                        message.Text.text = senderName + ": " + text;
                }
                else if (sender != null)
                    commandParser.TryParse(text, sender);
        }
    }
}