using Assets.Scripts.Entities.Effects;
using Assets.Scripts.Entities.Effects.Movement;
using Assets.Scripts.Movement.Scriptables;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    public abstract class MovementController : MonoBehaviour
    {
        [field: SerializeField] protected MovementData data { get; private set; }

        public List<MovementEffect> Effects { get; private set; } = new List<MovementEffect>();

        public delegate void MovementHandler(MovementData data, ref float speed, ref float flySpeed, ref float runSpeedBoost, ref float jumpPower);

        public event MovementHandler OnMove;
        public event Action<MovementEffect> OnEffectAdded;
        public event Action<MovementEffect> OnEffectRemoved;

        protected float flySpeed;
        protected float runSpeedBoost;
        protected float jumpPower;
        protected bool isCrouch;
        protected float currentWalkSpeed;

        public void ApplyEffect(MovementEffect effect)
        {
            if (!effect.isEnded)
            {
                effect.OnEffectEnded += HandleEffectEnd;
                Effects.Add(effect);
                OnEffectAdded?.Invoke(effect);
            }
        }
        public void RemoveEffect(Type type)
        {
            MovementEffect findedEffect = Effects.Find(n => n.GetType() == type);

            if (findedEffect != null)
            {
                findedEffect.StopEffect();
                Effects.Remove(findedEffect);
                OnEffectRemoved?.Invoke(findedEffect);
            }
        }

        protected void Move()
        {
            OnMove?.Invoke(data, ref currentWalkSpeed, ref flySpeed, ref runSpeedBoost, ref jumpPower);
        }

        private void HandleEffectEnd(Effect effect)
        {
            Effects.Remove(Effects.Find(n => n.GetType() == effect.GetType()));
        }
    }
}