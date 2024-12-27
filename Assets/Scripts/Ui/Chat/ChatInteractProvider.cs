using Assets.Scripts.Network.Chat;
using UnityEngine;

namespace Assets.Scripts.Ui.Chat
{
    [RequireComponent(typeof(ChatInputHandler))]
    public class ChatInteractProvider : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject player;

        private ChatInputHandler chat => GetComponent<ChatInputHandler>();

        public void Send(string message) => chat.HandleMessage(message, "me", player);
    }
}
