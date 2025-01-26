using Assets.Scripts.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Network.Chat
{
    public class KillCommand : Command
    {
        public override bool Execute(EntityNetworkData initiator, params string[] args)
        {
            if (args.Length == 0)
            {
                if (initiator.TryGetComponent<Entity>(out var initiatorEntity))
                {
                    initiatorEntity.Kill();
                    return true;
                }
                else
                {
                    ThrowMissingArgument(0, "entity");
                    return false;
                }
            }

            Selector selector = ParseSelector(0, args[0]);
            if (selector == null)
            {
                ThrowInvalidArgument(0, "selector");
                return false;
            }

            List<GameObject> targets = GetTarget(initiator.transform, selector);
            if (targets == null || targets.Count == 0)
            {
                ThrowMissingTarget(selector);
                return false;
            }

            foreach (GameObject target in targets)
            {
                if (target.TryGetComponent<Entity>(out var e))
                {
                    e.Kill();
                }
            }

            return true;
        }

        public override bool CanExecute(EntityNetworkData.Rights rights)
        {
            return rights > EntityNetworkData.Rights.Default;
        }
    }
}
