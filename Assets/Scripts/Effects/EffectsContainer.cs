using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Effects
{
    public class EffectContainer : MonoBehaviour
    {
        public List<Effect> Effects { get; private set; } = new List<Effect>();

        public event Action<Effect> OnEffectAdded;
        public event Action<Effect> OnEffectRemoved;

        public void ApplyEffect(Effect effect)
        {
            Effect findedEffect = Effects.Find(n => n.GetType() == effect.GetType());

            if (!effect.isEnded)
            {
                if (findedEffect != null)
                    findedEffect.CombineEffects(effect);
                else
                {
                    effect.OnEffectEnded += HandleEffectEnd;
                    Effects.Add(effect);
                    OnEffectAdded?.Invoke(effect);
                }
            }
        }
        public void RemoveEffect(Type type)
        {
            Effect findedEffect = Effects.Find(n => n.GetType() == type);

            if (findedEffect != null)
            {
                findedEffect.StopEffect();
                Effects.Remove(findedEffect);
                OnEffectRemoved?.Invoke(findedEffect);
            }
        }

        private void HandleEffectEnd(Effect effect)
        {
            Effects.Remove(Effects.Find(n => n.GetType() == effect.GetType()));
        }
    }
}