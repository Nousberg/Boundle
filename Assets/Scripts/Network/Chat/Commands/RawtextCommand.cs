using Assets.Scripts.Ui.Chat;
using Photon.Pun;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Network.Chat.Commands
{
    public class RawtextCommand : Command
    {
        protected override int cooldown => 2;

        protected override bool TryExecute(EntityNetworkData initiator, List<object> args)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (initiator.EntityRights < EntityNetworkData.Rights.Moderator)
                {
                    ThrowMissingRight(EntityNetworkData.Rights.Moderator, "trying to rawtext");
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
                ThrowMissingArgument(2, "data");
                return false;
            }
            else
            {
                if (args[0] is Selector selector)
                {
                    List<string> logTtargets = new List<string>();

                    foreach (var target in selector.targets)
                    {
                        if (target.TryGetComponent<PhotonView>(out var view) && target.TryGetComponent<EntityNetworkData>(out var networkData))
                            try
                            {
                                view.RPC(nameof(UiChatController.ShowMessageNative), view.Owner, args[0].ToString());
                                logTtargets.Add(networkData.Nametag);
                            }
                            catch (Exception ex)
                            {
                                ThrowInvalidArgument(1, ex.Message);
                                return false;
                            }
                    }

                    if (logTtargets.Count < 1)
                        ThrowNoTarget("rawtext");
                    else
                    {
                        SuccessExecution(logTtargets, "rawtext");
                        return true;
                    }

                    return false;
                }
            }

            ThrowInvalidArgument(1, "selector");
            return false;
        }
        public override List<CommandParser.Argument> GetSignature()
            => new List<CommandParser.Argument>() { new CommandParser.Argument(CommandParser.ArgumentType.Selector, true), new CommandParser.Argument(CommandParser.ArgumentType.String) };
    }
}
