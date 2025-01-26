using Assets.Scripts.Core.InputSystem;
using Assets.Scripts.Network;
using Assets.Scripts.Network.Chat;
using Assets.Scripts.Spawning;
using Assets.Scripts.Ui.Multiplayer;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private InputHandler input;
        [SerializeField] private ChatInputHandler chat;
        [SerializeField] private CommandParser cmd;
        [SerializeField] private PlayerValidator playerValidator;
        [SerializeField] private Summonables summoner;

        private void Start()
        {
            input.Init();
            playerValidator.Init(summoner);
            summoner.Init();
            chat.Init();
            cmd.Init();
        }
    }
}
