using System;
using UnityEngine;

namespace Assets.Scripts.Network.Chat
{
    public class RawtextCommand : Command
    {
        public override bool Execute(EntityNetworkData initiator, params string[] args)
        {
            if (CanExecute(initiator.EntityRights))
            {
                ChatInputHandler chat = UnityEngine.Object.FindObjectOfType<ChatInputHandler>();

                if (chat != null)
                {
                    try
                    {
                        Message buildedMessage = JsonUtility.FromJson<Message>(args[1]);
                        chat.HandleMessage(buildedMessage);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        ThrowInvalidArgument(1, ex.Message);
                    }
                }
            }

            return false;
        }
        public override bool CanExecute(EntityNetworkData.Rights rights) => rights > EntityNetworkData.Rights.Default;
    }
}
