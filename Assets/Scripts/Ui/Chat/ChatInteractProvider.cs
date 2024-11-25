using Assets.Scripts.Entities;
using Assets.Scripts.Network;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui.Chat
{
    [RequireComponent(typeof(Entity))]
    public class ChatInteractProvider : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ChatInputHandler chat;

        private Entity player => GetComponent<Entity>();

        public void Send(string message)
        {
            chat.HandleMessage(message, "me", player);
        }
    }
}
