using Assets.Scripts.Effects;
using Assets.Scripts.Entities;
using Assets.Scripts.Inventory;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Network.Chat.Commands
{
    public class EffectCommand : Command
    {
        protected override int cooldown => 2;

        protected override bool TryExecute(EntityNetworkData initiator, List<object> args)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (initiator.EntityRights < EntityNetworkData.Rights.Moderator)
                {
                    ThrowMissingRight(EntityNetworkData.Rights.Moderator, "trying to effect");
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

            string mode = args[1]?.ToString();
            string effectType = args[2]?.ToString();

            RemovableEffects targetEffects = new RemovableEffects(new List<Type>());
            if (!TryGetEffect(effectType, targetEffects))
                ThrowInvalidArgument(3, "type");
            
            List<string> logTargets = new List<string>();

            if (mode == "add")
            {
                if (args.Count < 5)
                {
                    ThrowMissingArgument(args.Count + 1, "required arguments for 'add'");
                    return false;
                }

                if (!int.TryParse(args[3]?.ToString(), out int duration))
                {
                    ThrowInvalidArgument(4, "duration");
                    return false;
                }

                if (!float.TryParse(args[4]?.ToString(), out float amplifier))
                {
                    ThrowInvalidArgument(5, "amplifier");
                    return false;
                }

                bool infinite = false;
                if (args.Count > 5 && !bool.TryParse(args[5]?.ToString(), out infinite))
                {
                    ThrowInvalidArgument(6, "infinite");
                    return false;
                }

                foreach (var target in selector.targets)
                    if (target.TryGetComponent<EntityNetworkData>(out var networkData) && ApplyEffect(target, effectType, duration, amplifier, infinite))
                        logTargets.Add(networkData.Nametag);
            }
            else if (mode == "remove")
            {
                foreach (var target in selector.targets)
                    if (target.TryGetComponent<EntityNetworkData>(out var networkData) && RemoveEffect(target, targetEffects))
                        logTargets.Add(networkData.Nametag);         
            }
            else
            {
                ThrowInvalidArgument(2, "mode");
                return false;
            }

            if (logTargets.Count > 0)
                SuccessExecution(logTargets, "effect");
            else
            {
                ThrowNoTarget("effect");
                return false;
            }

            return true;
        }

        public override List<CommandParser.Argument> GetSignature()
            => new List<CommandParser.Argument>() {
                new CommandParser.Argument(CommandParser.ArgumentType.Selector), 
                new CommandParser.Argument(CommandParser.ArgumentType.String), 
                new CommandParser.Argument(CommandParser.ArgumentType.String), 
                new CommandParser.Argument(CommandParser.ArgumentType.Int),
                new CommandParser.Argument(CommandParser.ArgumentType.Int),
                new CommandParser.Argument(CommandParser.ArgumentType.Bool) 
            };

        private bool TryGetEffect(string data, RemovableEffects dest)
        {
            switch (data)
            {
                case "godness":
                    dest.effects.Add(typeof(Godness));
                    return true;
                case "resistance":
                    dest.effects.Add(typeof(Resistance));
                    return true;
                case "supression":
                    dest.effects.Add(typeof(Supression));
                    return true;
                case "every":
                    dest.effects.AddRange(new List<Type>() { 
                        typeof(Godness),
                        typeof(Resistance),
                        typeof(Supression)
                    });
                    return true;
                default:
                    dest.effects = new List<Type>();
                    return false;
            }
        }
        private bool RemoveEffect(EntityNetworkData target, RemovableEffects targetEffects)
        {
            if (target.TryGetComponent<EffectContainer>(out var effects))
            {
                var toRemove = effects.Effects.Where(e => targetEffects.effects.Any(e1 => e.GetType() == e1)).ToList();

                if (toRemove.Count > 0)
                {
                    foreach (var effect in toRemove)
                        effects.RemoveEffectByType(effect.GetType());

                    return true;
                }
            }

            return false;
        }

        private bool ApplyEffect(EntityNetworkData target, string data, int duration, float amplifier, bool infinite)
        {
            if (target.TryGetComponent<EffectContainer>(out var effects))
                switch (data)
                {
                    case "godness":
                        if (target.TryGetComponent<Entity>(out var e))
                            effects.ApplyEffect(new Godness(e, duration, amplifier, infinite));
                        return true;
                    case "resistance":
                        if (target.TryGetComponent<Entity>(out var e1))
                            effects.ApplyEffect(new Resistance(e1, duration, amplifier, infinite));
                        return true;
                    case "supression":
                        if (target.TryGetComponent<InventoryDataController>(out var i))
                            effects.ApplyEffect(new Supression(i, duration, amplifier, infinite));
                        return true;
                    default:
                        ThrowInvalidArgument(3, "type");
                        return false;
                }

            return false;
        }

        private class RemovableEffects
        {
            public List<Type> effects;

            public RemovableEffects(List<Type> effects)
            {
                this.effects = effects;
            }
        }
    }
}
