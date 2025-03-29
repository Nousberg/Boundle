using Assets.Scripts.Inventory;
using Photon.Pun;
using System.Collections.Generic;

namespace Assets.Scripts.Network.Chat.Commands
{
    public class InventoryCommand : Command
    {
        protected override int cooldown => 2;

        protected override bool TryExecute(EntityNetworkData initiator, List<object> args)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (initiator.EntityRights < EntityNetworkData.Rights.Moderator)
                {
                    ThrowMissingRight(EntityNetworkData.Rights.Moderator, "trying to edit inventory");
                    return false;
                }
            }

            if (args.Count < 3)
            {
                ThrowMissingArgument(args.Count + 1, "required arguments");
                return false;
            }

            if (args[0] is not Selector selector)
            {
                ThrowInvalidArgument(1, "selector");
                return false;
            }

            string mode = args[1].ToString();

            List<string> logTargets = new List<string>();

            if (mode == "add")
            {
                if (args[2] is not int id)
                {
                    ThrowInvalidArgument(2, "identifier");
                    return false;
                }

                int amount = (args.Count > 3 && args[3] is int amt) ? amt : 1;
                if (amount < 1)
                {
                    ThrowInvalidArgument(4, "amount");
                    return false;
                }

                foreach (var target in selector.targets)
                {
                    if (target.TryGetComponent<InventoryDataController>(out var inv) && target.TryGetComponent<EntityNetworkData>(out var networkData))
                    {
                        bool result = true;
                        for (int i = 0; i < amount; i++)
                        {
                            if (!inv.TryAddItemByIndex(id))
                            {
                                result = false;
                                break;
                            }
                        }

                        if (result)
                            logTargets.Add(networkData.Nametag);
                    }
                }
            }
            else if (mode == "remove")
            {
                if (args[2] is not int index)
                {
                    ThrowInvalidArgument(3, "index");
                    return false;
                }

                int amount = (args.Count > 3 && args[3] is int amt) ? amt : 1;
                if (amount < 1)
                {
                    ThrowInvalidArgument(4, "amount");
                    return false;
                }

                foreach (var target in selector.targets)
                {
                    if (target.TryGetComponent<InventoryDataController>(out var inv) && target.TryGetComponent<EntityNetworkData>(out var networkData))
                    {
                        bool result = true;
                        int start = inv.GetItems.Count - index - 1;
                        int end = start - amount + 1;

                        for (int i = start; i >= end && i >= 0; i--)
                        {
                            if (!inv.TryRemoveItem(i))
                            {
                                result = false;
                                break;
                            }
                        }

                        if (result)
                            logTargets.Add(networkData.Nametag);
                    }
                }
            }

            if (logTargets.Count > 0)
                SuccessExecution(logTargets, "inventory");
            else
            {
                ThrowNoTarget("inventory");
                return false;
            }

            return true;
        }

        public override List<CommandParser.Argument> GetSignature()
            => new List<CommandParser.Argument>() {
                new CommandParser.Argument(CommandParser.ArgumentType.Selector),
                new CommandParser.Argument(CommandParser.ArgumentType.String),
                new CommandParser.Argument(CommandParser.ArgumentType.Int),
                new CommandParser.Argument(CommandParser.ArgumentType.Int)
            };
    }
}