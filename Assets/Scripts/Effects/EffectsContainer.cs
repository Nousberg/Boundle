using System;
using Photon.Pun;
using System.Collections.Generic;
using Assets.Scripts.Saving.EffectsData;
using UnityEngine;
using Assets.Scripts.Entities;
using Assets.Scripts.Inventory;

namespace Assets.Scripts.Effects
{
    public class EffectContainer : MonoBehaviour
    {
        public List<Effect> Effects { get; private set; } = new List<Effect>();

        public event Action<Effect> OnEffectAdded;
        public event Action<Effect> OnEffectRemoved;

        private PhotonView view => GetComponent<PhotonView>();

        [PunRPC]
        private void RPC_ApplyEffect(string effect)
        {
            if (view.IsMine)
            {
                SavedEffect data = JsonUtility.FromJson<SavedEffect>(effect);

                if (data.type == "Resistance" && TryGetComponent<Entity>(out var e))
                    ApplyEffect(new Resistance(e, data.duration, data.amplifier, data.infinite));
                else if (data.type == "Godness" && TryGetComponent<Entity>(out var e1))
                    ApplyEffect(new Godness(e1, data.duration, data.amplifier, data.infinite));
                else if (data.type == "Supression" && TryGetComponent<InventoryDataController>(out var i))
                    ApplyEffect(new Supression(i, data.duration, data.amplifier, data.infinite));
            }
        }
        [PunRPC]
        private void RPC_RemoveEffectByType(string type)
        {
            if (view.IsMine)
                foreach (Effect effect in Effects)
                    if (effect.GetType().Name == type)
                        RemoveEffectByType(effect.GetType());
        }
        [PunRPC]
        private void RPC_ClearEffects()
        {
            if (view.IsMine)
                foreach (Effect effect in Effects)
                    effect.StopEffect();
        }
        private void NativeClearEffects()
        {
            foreach (Effect effect in new List<Effect>(Effects))
                effect.StopEffect();
        }

        public void ClearEffects()
        {
            if (view.IsMine)
                foreach (Effect effect in new List<Effect>(Effects))
                    effect.StopEffect();
            else
                view.RPC(nameof(RPC_ClearEffects), view.Owner);
        }
        public void ApplyEffect(Effect effect)
        {
            if (view.IsMine)
            {
                if (!effect.IsEnded)
                {
                    Effect findedEffect = Effects.Find(n => n.GetType() == effect.GetType());

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
            else
                view.RPC(nameof(RPC_ApplyEffect), view.Owner,
                    JsonUtility.ToJson(
                        new SavedEffect(
                            effect.Duration, effect.Amplifier, effect.Infinite, effect.GetType().Name, -1)));
        }
        public void RemoveEffectByType(Type type)
        {
            if (view.IsMine)
            {
                Effect findedEffect = Effects.Find(n => n.GetType() == type);

                if (findedEffect != null)
                {
                    findedEffect.StopEffect();
                    Effects.Remove(findedEffect);
                    OnEffectRemoved?.Invoke(findedEffect);
                }
            }
            else
                view.RPC(nameof(RPC_RemoveEffectByType), view.Owner, type.Name);
        }
        public void RemoveEffect(Effect effect)
        {
            if (view.IsMine)
            {
                if (!Effects.Contains(effect))
                    return;

                Effects.Remove(effect);
                OnEffectRemoved?.Invoke(effect);
            }
        }

        private void HandleEffectEnd(Effect effect)
        {
            Effects.Remove(effect);
            OnEffectRemoved?.Invoke(effect);
        }
    }
}