using Assets.Scripts.Network.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class CommandParser : MonoBehaviour
    {
        private readonly Dictionary<string, Command> commands = new Dictionary<string, Command>()
        {
            { "kill", new KillCommand() },
            { "effect", new EffectCommand() },
            { "rawtext", new RawtextCommand() }
        };

        public const string COMMANDS_PREFIX = "/";
        public event Action<string> OnSuccessfulParse;
        public event Action<string> OnFailureParse;

        private ChatInputHandler chat => GetComponent<ChatInputHandler>();

        public void Init()
        {
            commands["kill"].OnSuccessExecution += (msg) => OnSuccessfulParse?.Invoke(msg);
            commands["kill"].OnFailureExecution += (msg) => OnFailureParse?.Invoke(msg);

            commands["effect"].OnSuccessExecution += (msg) => OnSuccessfulParse?.Invoke(msg);
            commands["effect"].OnFailureExecution += (msg) => OnFailureParse?.Invoke(msg);

            commands["rawtext"].OnSuccessExecution += (msg) => OnSuccessfulParse?.Invoke(msg);
            commands["rawtext"].OnFailureExecution += (msg) => OnFailureParse?.Invoke(msg);
        }

        public bool TryParse(string cmd, GameObject initiator)
        {
            EntityNetworkData eData = initiator.GetComponent<EntityNetworkData>();

            if (eData == null)
                return false;

            string[] instanceCmd = cmd
            .Replace(COMMANDS_PREFIX, string.Empty)
            .Trim()
            .ToLower()
            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (string.IsNullOrEmpty(instanceCmd[0]) || !commands.ContainsKey(instanceCmd[0]))
            {
                OnFailureParse?.Invoke("Undefined command " + instanceCmd[0]);
                return false;
            }

            List<string> lCmd = instanceCmd.ToList();
            lCmd.RemoveAt(0);

            return commands[instanceCmd[0]].Execute(eData, lCmd.ToArray());
        }
    }
}