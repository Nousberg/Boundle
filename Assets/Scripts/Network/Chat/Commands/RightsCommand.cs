using Photon.Pun;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Network.Chat.Commands
{
    internal class RightsCommand : Command
    {
        protected override int cooldown => 12;

        protected override bool TryExecute(EntityNetworkData initiator, List<object> args)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (initiator.EntityRights < EntityNetworkData.Rights.Absolute)
                {
                    ThrowMissingRight(EntityNetworkData.Rights.Absolute, "trying to set rights");
                    return false;
                }
            }

            if (args.Count < 1)
            {
                ThrowMissingArgument(1, "selector");
                return false;
            }
            else if (args.Count < 2)
            {
                ThrowMissingArgument(2, "right");
                return false;
            }

            if (Enum.TryParse<EntityNetworkData.Rights>(args[1].ToString(), true, out var right))
            {
                if (args[0] is not Selector selector)
                {
                    ThrowInvalidArgument(1, "selector");
                    return false;
                }

                List<string> logTargets = new List<string>();

                foreach (var target in selector.targets)
                    if (target.TryGetComponent<EntityNetworkData>(out var eData2))
                    {
                        if (eData2.SetRigths(initiator, right, PhotonNetwork.IsMasterClient))
                            logTargets.Add(eData2.Nametag);
                    }

                if (logTargets.Count > 0)
                {
                    SuccessExecution(logTargets, "right");
                    return true;
                }
            }
            else
            {
                ThrowInvalidArgument(2, "right");
                return false;
            }

            ThrowNoTarget("right");
            return false;
        }
        public override List<CommandParser.Argument> GetSignature()
            => new List<CommandParser.Argument>() {
                new CommandParser.Argument(CommandParser.ArgumentType.Selector),
                new CommandParser.Argument(CommandParser.ArgumentType.String)
            };
    }
}
