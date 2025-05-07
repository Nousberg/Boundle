using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Network.Chat.Commands
{
    public class TeleportCommand : Command
    {
        protected override int cooldown => 2;

        protected override bool TryExecute(EntityNetworkData initiator, List<object> args)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (initiator.EntityRights < EntityNetworkData.Rights.Moderator)
                {
                    ThrowMissingRight(EntityNetworkData.Rights.Moderator, "trying to teleport");
                    return false;
                }
            }

            if (args.Count < 4)
            {
                ThrowMissingArgument(args.Count + 1, "required arguments");
                return false;
            }

            if (args[0] is not Selector selector)
            {
                ThrowInvalidArgument(1, "selector");
                return false;
            }

            if (args[1] is not int x)
            {
                ThrowInvalidArgument(2, "vector x");
                return false;
            }
            if (args[2] is not int y)
            {
                ThrowInvalidArgument(3, "vector y");
                return false;
            }
            if (args[3] is not int z)
            {
                ThrowInvalidArgument(4, "vector z");
                return false;
            }

            List<string> logTargest = new List<string>();

            foreach (var target in selector.targets)
                if (target.TryGetComponent<EntityNetworkData>(out var networkData) && target.TryGetComponent<PhotonTransformView>(out var tT))
                {
                    tT.photonView.RPC(nameof(tT.SetPosition), tT.photonView.Owner, new Vector3(x, y, z));
                    logTargest.Add(networkData.Nametag);
                }

            if (logTargest.Count < 1)
            {
                ThrowNoTarget("teleport");
                return false;
            }

            SuccessExecution(logTargest, "teleport");
            return true;
        }

        public override List<CommandParser.Argument> GetSignature()
            => new List<CommandParser.Argument>() {
                new CommandParser.Argument(CommandParser.ArgumentType.Selector),
                new CommandParser.Argument(CommandParser.ArgumentType.Int),
                new CommandParser.Argument(CommandParser.ArgumentType.Int),
                new CommandParser.Argument(CommandParser.ArgumentType.Int),
            };
    }
}
