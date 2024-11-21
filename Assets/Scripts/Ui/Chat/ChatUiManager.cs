using Assets.Scripts.Entities;
using Assets.Scripts.Network;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui.Chat
{
    [RequireComponent(typeof(Entity))]
    public class ChatUiManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ChatInputHandler chat;
        [SerializeField] private TMP_InputField textBox;

        private Entity player => GetComponent<Entity>();

        private void Start()
        {
            textBox.onEndEdit.AddListener(HandleEndEdit);
        }
        private void HandleEndEdit(string message)
        {
            chat.HandleMessage(message, "t", player);
        }
    }
}
