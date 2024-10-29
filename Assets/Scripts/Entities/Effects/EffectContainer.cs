using Assets.Scripts.Entities.Effects.Interfaces;
using Assets.Scripts.Movement.Scriptables;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Assets.Scripts.Entities.Effects
{
    public class EffectContainer : MonoBehaviour
    {
        public List<Effect> Effects { get; private set; } = new List<Effect>();

        private void Update()
        {
            foreach (var e in Effects)
            {
                if (e.Duration <= 0f && !e.Infinite)
                {
                    Effects.Remove(e);
                    continue;
                }
                e.SetDuration(e.Duration - Time.deltaTime);
            }
        }

        public void CalculateDamage(ref float amount, Entity attacker, DamageType type)
        {
            foreach (var e in Effects)
                if (e is IDamageEffect<Effect> damageEffect)
                    damageEffect.ModifyDamage(ref amount, attacker, type);
        }
        public void CalculateMovement(MovementData data, ref float speed, ref float flySpeed, ref float runSpeedBoost, ref float jumpPower)
        {
            foreach (var e in Effects)
                if (e is IMovementEffect<Effect> movementEffect)
                    movementEffect.ModifyMovement(data, ref speed, ref flySpeed, ref runSpeedBoost, ref jumpPower);
        }
        public void ApplyEffect(Effect effect)
        {
            if (Effects.Contains(effect))
            {
                effect.SetDuration(effect.Duration + effect.Duration);

                if (effect is Resistance resistance)
                    resistance.SetAmplifier(resistance.Amplifier + resistance.Amplifier);
            }
        }
        public void RemoveEffect(int index)
        {
            if (index > 0 || index >= Effects.Count)
                return;

            Effects.RemoveAt(index);
        }
    }
}