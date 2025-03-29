using Cysharp.Threading.Tasks;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Network.Chat.Commands
{
    public abstract class Command
    {
        protected abstract int cooldown { get; }

        public event Action<string> OnSuccessExecution;
        public event Action<string> OnFailureExecution;

        private float remainingCooldown;

        public bool Execute(EntityNetworkData initiator, List<object> args)
        {
            if (!PhotonNetwork.IsMasterClient)
                if (remainingCooldown > 0f)
                {
                    OnFailureExecution?.Invoke($"Too fast, try again in {remainingCooldown}");
                    return false;
                }

            bool result = TryExecute(initiator, args);

            if (result)
            {
                remainingCooldown = cooldown;
                _ = CommandCooldown();
            }
            return result;
        }
        protected abstract bool TryExecute(EntityNetworkData initiator, List<object> args);
        public abstract List<CommandParser.Argument> GetSignature();

        protected void SuccessExecution(List<string> targets, string context) => OnSuccessExecution($"Succesfuly executed {context} on {string.Join(", ", targets)}");
        protected void ThrowMissingRight(EntityNetworkData.Rights right, string context) => OnFailureExecution($"Missing {right.ToString()} right while {context}");
        protected void ThrowMissingArgument(int argument, string context) => OnFailureExecution?.Invoke($"Missing {context} at {argument}");
        protected void ThrowInvalidArgument(int argument, string context) => OnFailureExecution?.Invoke($"Invalid \"{context}\" at {argument}");
        protected void ThrowNoTarget(string context) => OnFailureExecution?.Invoke($"No target match selector while {context}");

        private async UniTask CommandCooldown()
        {
            int interval = 1000;

            while (remainingCooldown > 0f)
            {
                await UniTask.Delay(interval);
                remainingCooldown -= interval / 1000;
            }
        }
    }
}
