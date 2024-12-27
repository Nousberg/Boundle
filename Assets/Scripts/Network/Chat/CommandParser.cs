using Assets.Scripts.Effects;
using Assets.Scripts.Entities;
using Assets.Scripts.Inventory;
using System;
using UnityEngine;

namespace Assets.Scripts.Network.Chat
{
    public class CommandParser : MonoBehaviour
    {
        public event Action<string> OnSuccessfulParse;
        public event Action<string> OnFailureParse;

        public bool TryParse(string cmd, GameObject initiator)
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
                    EffectContainer effects = initiator.GetComponent<EffectContainer>();
                    Entity target = initiator.GetComponent<Entity>();

                    if (effects == null || target == null)
                        return false;

                    if (instanceCmd[1] == "add")
                    {
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
                                OnFailureParse?.Invoke("Unexpected value at input parameter");
                                return false;
                            }
                    
                            if (instanceCmd[2] == "resistance")
                            {
                                effects.ApplyEffect(new Resistance(target, duration, amplifier, infinite));
                                OnSuccessfulParse?.Invoke("Applied effect");
                                return true;
                            }
                            else if (instanceCmd[2] == "godness")
                            {
                                effects.ApplyEffect(new Godness(target, duration, amplifier, infinite));
                                OnSuccessfulParse?.Invoke("Applied effect");
                                return true;
                            }
                            else if (instanceCmd[2] == "supression")
                            {
                                InventoryDataController inventory = initiator.GetComponent<InventoryDataController>();

                                if (inventory != null)
                                    effects.ApplyEffect(new Supression(inventory, duration, amplifier, infinite));
                            }
                        }
                    }
                    else if (instanceCmd[1] == "remove")
                        if (instanceCmd[2] == "resistance")
                        {
                            effects.RemoveEffect(typeof(Resistance));
                            OnSuccessfulParse?.Invoke("Removed effect");
                            return true;
                        }
                        else if (instanceCmd[2] == "godness")
                        {
                            effects.RemoveEffect(typeof(Godness));
                            OnSuccessfulParse?.Invoke("Removed effect");
                            return true;
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
                            OnFailureParse?.Invoke("Unexpected value at input parameter");
                            return false;
                        }

                        foreach (var source in FindObjectsOfType<DamageSourcePrototype>())
                        {
                            source.damageAmount = amount;
                            source.damageType = damageType;
                            source.damageFrequency = frequency;
                        }
                        OnSuccessfulParse?.Invoke("Changes applied");
                        return true;
                    }
                    break;
            }
            OnFailureParse?.Invoke("Unknown command");
            return false;
        }
    }
}
