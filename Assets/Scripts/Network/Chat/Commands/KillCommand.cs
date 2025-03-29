using Assets.Scripts.Entities;
using Photon.Pun;
using System.Collections.Generic;

namespace Assets.Scripts.Network.Chat.Commands
{
    public class KillCommand : Command
    {
        protected override int cooldown => 2;

        protected override bool TryExecute(EntityNetworkData initiator, List<object> args)
        {
            EntityNetworkData eData = initiator.GetComponent<EntityNetworkData>();

            if (!PhotonNetwork.IsMasterClient)
            {
                if (initiator.EntityRights < EntityNetworkData.Rights.Moderator)
                {
                    ThrowMissingRight(EntityNetworkData.Rights.Moderator, "trying to kill");
                    return false;
                }
            }

            if (args.Count < 1)
            {
                if (initiator.TryGetComponent<Entity>(out var e))
                {
                    e.Kill();
                    SuccessExecution(new List<string>() { eData.Nametag }, "kill");
                    return true;
                }
            }
            else
            {
                if (args[0] is Selector selector)
                {
                    List<string> logTargets = new List<string>();

                    foreach (var target in selector.targets)
                    {
                        if (target.TryGetComponent<EntityNetworkData>(out var networkData) && target.TryGetComponent<Entity>(out var e))
                        {
                            e.Kill();
                            logTargets.Add(networkData.Nametag);
                        }
                    }

                    if (logTargets.Count > 0)
                        SuccessExecution(logTargets, "kill");
                    else
                    {
                        ThrowNoTarget("kill");
                        return false;
                    }

                    return true;
                }
            }

            ThrowInvalidArgument(1, "selector");
            return false;
        }
        public override List<CommandParser.Argument> GetSignature()
            => new List<CommandParser.Argument>() { new CommandParser.Argument(CommandParser.ArgumentType.Selector) };
    }
}
