using Assets.Scripts.Entities;
using Photon.Pun;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Network.Chat.Commands
{
    public class DamageCommand : Command
    {
        protected override int cooldown => 2;

        protected override bool TryExecute(EntityNetworkData initiator, List<object> args)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (initiator.EntityRights < EntityNetworkData.Rights.Moderator)
                {
                    ThrowMissingRight(EntityNetworkData.Rights.Moderator, "trying to damage");
                    return false;
                }
            }

            if (args.Count < 4)
            {
                ThrowMissingArgument(args.Count + 1, "required arguments");
                return false;
            }

            if (args[0] is not Selector targets)
            {
                ThrowInvalidArgument(1, "selector");
                return false;
            }
            if (args[1] is not Selector selector)
            {
                ThrowInvalidArgument(2, "selector");
                return false;
            }
            if (args[2] is not int amount)
            {
                ThrowInvalidArgument(3, "amount");
                return false;
            }

            if (selector.targets.Count < 1)
            {
                ThrowNoTarget("damage");
                return false;
            }

            DamageData.DamageType damageType;

            if (!Enum.TryParse<DamageData.DamageType>(args[3].ToString(), true, out damageType))
            {
                ThrowInvalidArgument(4, "type");
                return false;
            }

            Entity causer = selector.targets[0].GetComponent<Entity>();

            if (causer == null)
                return false;

            List<string> logTargets = new List<string>();

            foreach (var target in targets.targets)
            {
                if (target.TryGetComponent<EntityNetworkData>(out var networkData) && target.TryGetComponent<Entity>(out var e))
                {
                    e.TakeDamage(amount, causer, damageType);
                    logTargets.Add(networkData.Nametag);
                }
            }

            if (logTargets.Count < 1)
            {
                ThrowNoTarget("damage");
                return false;
            }

            SuccessExecution(logTargets, "damage");
            return true;
        }

        public override List<CommandParser.Argument> GetSignature()
            => new List<CommandParser.Argument>() {
                new CommandParser.Argument(CommandParser.ArgumentType.Selector),
                new CommandParser.Argument(CommandParser.ArgumentType.Selector),
                new CommandParser.Argument(CommandParser.ArgumentType.Int),
                new CommandParser.Argument(CommandParser.ArgumentType.String),
            };
    }
}
