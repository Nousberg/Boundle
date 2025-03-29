using Assets.Scripts.Entities;
using Assets.Scripts.Network.Chat;
using Assets.Scripts.Network.Chat.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class CommandParser : MonoBehaviour
    {
        public Dictionary<string, Command> Commands = new Dictionary<string, Command>();

        public const string COMMANDS_PREFIX = "/";
        public event Action<string> OnSuccessfulParse;
        public event Action<string> OnFailureParse;

        private ChatInputHandler chat => GetComponent<ChatInputHandler>();

        public void Init()
        {
            Commands.Add("kill", new KillCommand());
            Commands.Add("effect", new EffectCommand());
            Commands.Add("rawtext", new RawtextCommand());
            Commands.Add("kick", new KickCommand());
            Commands.Add("rights", new RightsCommand());
            Commands.Add("inventory", new InventoryCommand());
            Commands.Add("damage", new DamageCommand());
            Commands.Add("tp", new TeleportCommand());

            foreach (var command in Commands.Values)
            {
                command.OnFailureExecution += (ctx) => OnFailureParse?.Invoke(ctx);
                command.OnSuccessExecution += (ctx) => OnSuccessfulParse?.Invoke(ctx);
            }
        }

        public bool TryParse(string cmd, GameObject initiator)
        {
            EntityNetworkData eData = initiator.GetComponent<EntityNetworkData>();

            if (eData == null)
                return false;

            List<string> splittedCmd = cmd.Remove(0, COMMANDS_PREFIX.Length).Split(' ').ToList();

            if (splittedCmd.Count < 1 || !Commands.ContainsKey(splittedCmd[0]))
            {
                OnFailureParse("Undefined command \"" + (splittedCmd.Count < 1 || string.IsNullOrEmpty(splittedCmd[0]) ? "null" : splittedCmd[0]) + "\"");
                return false;
            }

            string command = splittedCmd[0];
            splittedCmd.RemoveAt(0);

            List<object> args = new List<object>();

            foreach (string arg in splittedCmd)
            {
                int argIndex = splittedCmd.IndexOf(arg);

                List<Argument> cmdArgs = Commands[command].GetSignature();
                Argument cmdArg = cmdArgs[splittedCmd.IndexOf(arg)];

                args.Add(ParseArgument(eData, cmdArg, arg));

                if (cmdArg.disableFormattingAfterThis)
                {
                    List<string> noFormatting = new List<string>(splittedCmd);
                    noFormatting.RemoveRange(0, argIndex + 1);

                    args.Add(string.Join(' ', noFormatting));
                    break;
                }
                if (argIndex > cmdArgs.Count - 1 && !cmdArg.disableFormattingAfterThis)
                    break;
            }

            return Commands[command].Execute(eData, args);
        }

        private object ParseArgument(EntityNetworkData initiator, Argument arg, string value)
        {
            if (arg.type == ArgumentType.Int && int.TryParse(value, out var i))
                return i;
            if (arg.type == ArgumentType.Bool && bool.TryParse(value, out var b))
                return b;
            if (arg.type == ArgumentType.Selector && TryParseSelector(initiator, value, out var s))
                return s;

            return value;
        }
        private bool TryParseSelector(EntityNetworkData initiator, string data, out Selector selector)
        {
            selector = new Selector(new List<EntityNetworkData>());
            var allObjects = FindObjectsOfType<EntityNetworkData>().ToList();
            var regex = new Regex(@"^(?<filter>\w+)(\[(?<conditions>[^\]]+)\])?$");

            var match = regex.Match(data);
            if (!match.Success)
            {
                OnFailureParse?.Invoke($"Invalid selector format: {data}");
                selector = null;
                return false;
            }

            string filter = match.Groups["filter"].Value;
            string conditions = match.Groups["conditions"].Value;

            switch (filter)
            {
                case "all":
                    selector.targets = allObjects;
                    break;
                case "players":
                    selector.targets = allObjects.Where(obj => obj.GetComponent<PlayerNetworkManager>() != null).ToList();
                    break;
                case "others":
                    selector.targets = allObjects.Where(obj => obj.gameObject.GetInstanceID() != initiator.gameObject.GetInstanceID()).ToList();
                    break;
                case "self":
                    selector.targets = new List<EntityNetworkData> { initiator };
                    break;
                case "nearest":
                    EntityNetworkData nearest = allObjects
                        .Where(obj => obj.gameObject.GetInstanceID() != initiator.gameObject.GetInstanceID())
                        .OrderBy(obj => Vector3.Distance(initiator.transform.position, obj.gameObject.transform.position))
                        .FirstOrDefault();

                    selector.targets = nearest != null ? new List<EntityNetworkData> { nearest } : new List<EntityNetworkData> { initiator };
                    break;
                default:
                    OnFailureParse?.Invoke($"Undefined filter \"{filter}\"");
                    selector = null;
                    return false;
            }

            if (!string.IsNullOrEmpty(conditions))
            {
                var conditionRegex = new Regex(@"(?<key>\w+)(?<operator>=!?|>|<)\((?<values>[^)]+)\)");
                var conditionMatches = conditionRegex.Matches(conditions);

                foreach (Match conditionMatch in conditionMatches)
                {
                    string key = conditionMatch.Groups["key"].Value;
                    string operatorType = conditionMatch.Groups["operator"].Value;
                    string[] values = conditionMatch.Groups["values"].Value
                        .Split(',')
                        .Select(v => v.Trim())
                        .ToArray();

                    bool isNegative = operatorType == "=!";

                    selector.targets = selector.targets.Where(obj =>
                    {
                        bool matches = false;
                        switch (key)
                        {
                            case "radius":
                                if (values.Length == 1 && float.TryParse(values[0], out var radius))
                                {
                                    float distance = Vector3.Distance(initiator.transform.position, obj.transform.position);
                                    matches = operatorType switch
                                    {
                                        "=" => distance == radius,
                                        ">" => distance > radius,
                                        "<" => distance < radius,
                                        _ => false
                                    };
                                }
                                break;
                            case "type":
                                matches = values.Any(value =>
                                {
                                    var entity = obj.GetComponent<Entity>();
                                    return entity != null && Enum.TryParse(typeof(EntityType), value.Trim('"'), true, out var parsedType) && entity.MyType == (EntityType)parsedType;
                                });
                                break;
                            case "nametag":
                                matches = values.Any(value =>
                                {
                                    var networkData = obj.GetComponent<EntityNetworkData>();
                                    return networkData != null && networkData.Nametag == value.Trim('"');
                                });
                                break;
                            case "health":
                                matches = values.Any(value =>
                                {
                                    if (int.TryParse(value, out var health))
                                    {
                                        var entity = obj.GetComponent<Entity>();
                                        if (entity != null)
                                        {
                                            int entityHealth = Mathf.RoundToInt(entity.Health);
                                            return operatorType switch
                                            {
                                                "=" => entityHealth == health,
                                                ">" => entityHealth > health,
                                                "<" => entityHealth < health,
                                                _ => false
                                            };
                                        }
                                    }
                                    return false;
                                });
                                break;
                            default:
                                OnFailureParse?.Invoke($"Undefined subfilter \"{key}\"");
                                return false;
                        }

                        return isNegative ? !matches : matches;
                    }).ToList();
                }
            }

            return true;
        }


        public enum ArgumentType : byte
        {
            Selector,
            String,
            Bool,
            Int
        }

        public class Argument
        {
            public ArgumentType type;
            public bool disableFormattingAfterThis;

            public Argument(ArgumentType type, bool disableFormattingAfterThis = false)
            {
                this.type = type;
                this.disableFormattingAfterThis = disableFormattingAfterThis;
            }
        }
    }
}