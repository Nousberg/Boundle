using System;

namespace Assets.Scripts.Network.Chat
{
    [Serializable]
    public class Message
    {
        public string Id;
        public string Author;
        public string Content;

        public bool IgnoreFilter;
        public bool LogMessage;

        public Message(string id, string author, string content, bool logMessage = false, bool ignoreFilter = false)
        {
            Id = id;
            Author = author;
            Content = content;
            LogMessage = logMessage;
            IgnoreFilter = ignoreFilter;
        }
    }
}
