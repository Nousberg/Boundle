using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Effects;
using Assets.Scripts.Network;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(ChatInputHandler))]
    public class CommandParser : MonoBehaviour
    {
        private ChatInputHandler chat => GetComponent<ChatInputHandler>();

        public bool TryParse(string cmd, Entity initiator)
        {
            string[] instanceCmd = cmd
                .Replace("/", string.Empty)
                .Trim()
                .ToLower()
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (instanceCmd.Length == 0)
                return false;

            switch (instanceCmd[0])
            {
                case "effect":
                    if (instanceCmd.Length > 5)
                    {
                        float amplifier;
                        int duration;
                        bool infinite;

                        try
                        {
                            duration = int.Parse(instanceCmd[3]);
                            amplifier = float.Parse(instanceCmd[4]);
                            infinite = bool.Parse(instanceCmd[5]);
                        }
                        catch
                        {
                            return false;
                        }

                        if (instanceCmd[1] == "add")
                            if (instanceCmd[2] == "resistance")
                            {
                                initiator.ApplyEffect(new Resistance(initiator, duration, amplifier, infinite));
                                chat.HandleMessage("Applied " + instanceCmd[2], string.Empty);
                                return true;
                            }
                            else if (instanceCmd[2] == "godness")
                            {
                                initiator.ApplyEffect(new Godness(initiator, duration, amplifier, infinite));
                                chat.HandleMessage("Applied " + instanceCmd[2], string.Empty);
                                return true;
                            }
                            else if (instanceCmd[2] == "remove")
                                if (instanceCmd[2] == "resistance")
                                {
                                    initiator.RemoveEffect(typeof(Resistance));
                                    chat.HandleMessage("Removed " + instanceCmd[2], string.Empty);
                                    return true;
                                }
                                else if (instanceCmd[2] == "godness")
                                {
                                    initiator.RemoveEffect(typeof(Godness));
                                    chat.HandleMessage("Removed " + instanceCmd[2], string.Empty);
                                    return true;
                                }
                    }
                    break;

                case "damage":
                    if (instanceCmd.Length > 3)
                    {
                        DamageType damageType;
                        float frequency;
                        float amount;

                        try
                        {
                            amount = float.Parse(instanceCmd[1]);
                            frequency = float.Parse(instanceCmd[2]);
                            damageType = Enum.Parse<DamageType>(instanceCmd[3], true);
                        }
                        catch
                        {
                            return false;
                        }

                        foreach (var source in FindObjectsOfType<DamageSourcePrototype>())
                        {
                            source.damageAmount = amount;
                            source.damageType = damageType;
                            source.damageFrequency = frequency;
                        }
                    }
                    break;
            }
            return false;
        }
    }
}
