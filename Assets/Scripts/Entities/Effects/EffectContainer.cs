using Assets.Scripts.Entities.Effects.Interfaces;
using Assets.Scripts.Movement.Scriptables;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Assets.Scripts.Entities.Effects
{
    public class EffectContainer : MonoBehaviour
    {
        public List<Effect> Effects { get; private set; } = new List<Effect>();
        private List<Effect> effectsToRemove = new List<Effect>();

        private void Update()
        {
            foreach (var e in Effects)
            {
                if (e.Duration <= 0f && !e.Infinite)
                {
                    Debug.Log(e.Infinite);
                    effectsToRemove.Add(e);
                    continue;
                }
                if (!e.Infinite)
                    e.SetDuration(e.Duration - Time.deltaTime);
            }
            if (effectsToRemove.Count > 0)
            {
                Effects.RemoveAll(e => effectsToRemove.Contains(e));
                effectsToRemove.Clear();
            }
        }

        public void CalculateDamage(ref float amount, Entity attacker, DamageType type)
        {
            if (Effects.Count > 0f)
                foreach (var e in Effects)
                    if (e is IDamageEffect<Effect> damageEffect)
                        damageEffect.ModifyDamage(ref amount, attacker, type);
        }
        public bool CalculateMovement(MovementData data, ref float speed, ref float flySpeed, ref float runSpeedBoost, ref float jumpPower)
        {
            if (Effects.Count > 0)
            {
                foreach (var e in Effects)
                    if (e is IMovementEffect<Effect> movementEffect)
                        movementEffect.ModifyMovement(data, ref speed, ref flySpeed, ref runSpeedBoost, ref jumpPower);

                return true;
            }
            else
            {
                return false; 
            }
        }
        public void ApplyEffect(Effect effect)
        {
            Type type = effect.GetType();
            Effect e = Effects.Find(n => n.GetType() == type);

            if (e != null)
            {
                e.SetDuration(effect.Duration + e.Duration);

                if (e is Resistance resistance && effect is Resistance resistance2)
                    resistance.SetAmplifier(resistance.Amplifier + resistance2.Amplifier);
            }
            else
            {
                Effects.Add(effect);
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