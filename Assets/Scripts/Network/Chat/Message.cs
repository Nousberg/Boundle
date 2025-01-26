using System;
using Assets.Scripts.Saving.Data;

namespace Assets.Scripts.Network.Chat
{
    [Serializable]
    public class Message
    {
        public Guid Id;
        public string Author;
        public string Content;

        public bool LogMessage;
        public Color ContentColor;

        public Message(Guid id, string author, string content, Color contentColor = default, bool logMessage = false)
        {
            Id = id;
            Author = author;
            Content = content;
            ContentColor = contentColor;
            LogMessage = logMessage;
        }
    }
}
