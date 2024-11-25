using Assets.Scripts.Entities.Effects;
using Assets.Scripts.Inventory.Scriptables;
using System;
using UnityEngine;
using Assets.Scripts.Entities.Effects.Inventory;

namespace Assets.Scripts.Inventory
{
    public class ItemDataController : MonoBehaviour
    {
        [field: SerializeField] public BaseItemData Data { get; private set; }

        public event Action<ItemEffect> OnEffectAdded;
        public event Action<ItemEffect> OnEffectRemoved;

        public DynamicItemData GetData => data;

        protected DynamicItemData data;

        public void InjectData(DynamicItemData data) => this.data = data;

        public void ApplyEffect(ItemEffect effect)
        {
            if (!effect.isEnded)
            {
                effect.OnEffectEnded += HandleEffectEnd;
                data.effects.Add(effect);
                OnEffectAdded?.Invoke(effect);
            }
        }
        public void RemoveEffect(Type type)
        {
            ItemEffect findedEffect = data.effects.Find(n => n.GetType() == type);

            if (findedEffect != null)
            {
                findedEffect.StopEffect();
                data.effects.Remove(findedEffect);
                OnEffectRemoved?.Invoke(findedEffect);
            }
        }

        private void HandleEffectEnd(Effect effect)
        {
            data.effects.Remove(data.effects.Find(n => n.GetType() == effect.GetType()));
        }
    }
}