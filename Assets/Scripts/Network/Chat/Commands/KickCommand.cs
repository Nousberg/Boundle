using Photon.Pun;
using System.Collections.Generic;

namespace Assets.Scripts.Network.Chat.Commands
{
    public class KickCommand : Command
    {
        protected override int cooldown => 12;

        protected override bool TryExecute(EntityNetworkData initiator, List<object> args)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                ThrowMissingRight(EntityNetworkData.Rights.Host, "trying to kick");
                return false;
            }

            if (args.Count < 1)
            {
                ThrowMissingArgument(1, "selector");
                return false;
            }

            if (args[0] is not Selector selector)
            {
                ThrowInvalidArgument(1, "selector");
                return false;
            }

            string kickReason = args.Count > 1 ? args[1].ToString() : "nothing";

            List<string> logTargets = new List<string>();

            foreach (var target in selector.targets)
                if (target != initiator && target.TryGetComponent<PhotonView>(out var view) && target.TryGetComponent<PlayerNetworkManager>(out var playerData) && target.TryGetComponent<EntityNetworkData>(out var entityData) && PhotonNetwork.IsMasterClient)
                {
                    view.RPC(nameof(PlayerNetworkManager.Kick), RpcTarget.All, kickReason, false, false);
                    logTargets.Add(entityData.Nametag);
                }

            if (logTargets.Count > 0)
                SuccessExecution(logTargets, "kick");
            else
            {
                ThrowNoTarget("kick");
                return false;
            }

            return true;
        }
        public override List<CommandParser.Argument> GetSignature()
            => new List<CommandParser.Argument>() { 
                new CommandParser.Argument(CommandParser.ArgumentType.Selector, true),
                new CommandParser.Argument(CommandParser.ArgumentType.String)
            };
    }
}
