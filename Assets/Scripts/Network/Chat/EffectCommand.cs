using Assets.Scripts.Effects;
using Assets.Scripts.Entities;
using UnityEngine;
using System.Collections.Generic;
using System;
using Assets.Scripts.Inventory;

namespace Assets.Scripts.Network.Chat
{
    public class EffectCommand : Command
    {
        public override bool Execute(EntityNetworkData initiator, params string[] args)
        {
            if (!CanExecute(initiator.EntityRights))
                return false;

            if (args.Length < 3)
            {
                ThrowMissingArgument(0, "data");
                return false;
            }

            string mode = args[0];
            if (mode != "add" && mode != "remove")
            {
                ThrowInvalidArgument(0, "mode");
                return false;
            }

            Selector selector = ParseSelector(1, args[1]);
            if (selector == null)
            {
                ThrowInvalidArgument(1, "selector");
                return false;
            }

            List<GameObject> targets = GetTarget(initiator.transform, selector);
            if (targets.Count == 0)
            {
                ThrowMissingTarget(selector);
                return false;
            }

            string effectType = args[2];

            int duration = args.Length > 3 ? int.Parse(args[3]) : 10;
            float amplifier = args.Length > 4 ? float.Parse(args[4]) : 1f;
            bool infinite = args.Length > 5 && bool.Parse(args[5]);

            foreach (GameObject target in targets)
            {
                if (target.TryGetComponent<Entity>(out Entity entity))
                {
                    var effectContainer = target.GetComponent<EffectContainer>();
                    if (effectContainer == null)
                        continue;

                    if (mode == "add")
                    {
                        Effect effectInstance = CreateEffect(effectType, entity, duration, amplifier, infinite);
                        if (effectInstance != null)
                        {
                            effectContainer.ApplyEffect(effectInstance);
                        }
                        else
                        {
                            ThrowInvalidArgument(2, $"effect {effectType}");
                            return false;
                        }
                    }
                    else if (mode == "remove")
                    {
                        Type effect = GetEffectType(effectType);
                        if (effect != null)
                        {
                            effectContainer.RemoveEffect(effect);
                        }
                        else
                        {
                            ThrowInvalidArgument(2, $"effect {effectType}");
                            return false;
                        }
                    }
                }
            }

            SuccessExecution((mode == "add" ? "Applied" : "Removed") + $"effect '{effectType}' to {targets.Count} targets.");
            return true;
        }

        public override bool CanExecute(EntityNetworkData.Rights rights)
        {
            bool result = rights > EntityNetworkData.Rights.Default;
            return result;
        }

        private Effect CreateEffect(string effectType, Entity target, int duration, float amplifier, bool infinite)
        {
            switch (effectType)
            {
                case "godness":
                    return new Godness(target, duration, amplifier, infinite);
                case "resistance":
                    return new Resistance(target, duration, amplifier, infinite);
                case "supression":
                    return new Supression(target.GetComponent<InventoryDataController>(), duration, amplifier, infinite);
                default:
                    return null;
            }
        }

        private Type GetEffectType(string data)
        {
            switch (data)
            {
                case "godness":
                    return typeof(Godness);
                case "resistance":
                    return typeof(Resistance);
                case "supression":
                    return typeof(Supression);
                default:
                    return null;
            }
        }
    }
}
