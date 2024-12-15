using Assets.Scripts.Inventory.Scriptables;
using Assets.Scripts.Inventory.DynamicData;
using System;
using UnityEngine;
using Assets.Scripts.Effects;

namespace Assets.Scripts.Inventory
{
    public class ItemDataController : MonoBehaviour
    {
        [SerializeField] protected Animator handsAnimator;
        [SerializeField] protected Animator itemAnimator;

        [SerializeField] protected InventoryDataController inventory { get; private set; }
        [field: SerializeField] public BaseItemData BaseData { get; private set; }

        public event Action<Effect> OnEffectAdded;
        public event Action<Effect> OnEffectRemoved;

        public DynamicItemData GetData => data;

        protected DynamicItemData data;

        public virtual void InjectData(DynamicItemData data) => this.data = data;

        public void ApplyEffect(Effect effect)
        {
            Effect findedEffect = data.effects.Find(n => n.GetType() == effect.GetType());

            if (!effect.isEnded)
            {
                if (findedEffect != null)
                    findedEffect.CombineEffects(effect);
                else
                {
                    effect.OnEffectEnded += HandleEffectEnd;
                    data.effects.Add(effect);
                    OnEffectAdded?.Invoke(effect);
                }
            }
        }
        public void RemoveEffect(Type type)
        {
            Effect findedEffect = data.effects.Find(n => n.GetType() == type);

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