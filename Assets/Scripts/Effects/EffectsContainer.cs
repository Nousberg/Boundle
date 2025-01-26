using System;
using Photon.Pun;
using System.Collections.Generic;
using Assets.Scripts.Saving;
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
                EffectData data = JsonUtility.FromJson<EffectData>(effect);

                if (data.type == "Resistance" && TryGetComponent<Entity>(out var e))
                    ApplyEffect(new Resistance(e, data.duration, data.amplifier, data.infinite));
                else if (data.type == "Godness" && TryGetComponent<Entity>(out var e1))
                    ApplyEffect(new Godness(e1, data.duration, data.amplifier, data.infinite));
                else if (data.type == "Supression" && TryGetComponent<InventoryDataController>(out var i))
                    ApplyEffect(new Supression(i, data.duration, data.amplifier, data.infinite));
            }
        }
        [PunRPC]
        private void RPC_RemoveEffect(string type)
        {
            if (view.IsMine)
                foreach (Effect effect in Effects)
                    if (effect.GetType().Name == type)
                        RemoveEffect(effect.GetType());
        }

        public void ApplyEffect(Effect effect)
        {
            if (view.IsMine)
            {
                Effect findedEffect = Effects.Find(n => n.GetType() == effect.GetType());

                if (!effect.IsEnded)
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
            else
                view.RPC("RPC_ApplyEffect", RpcTarget.All,
                    JsonUtility.ToJson(
                        new EffectData(
                            effect.Duration, effect.Amplifier, effect.Infinite, effect.GetType().Name)));
        }
        public void RemoveEffect(Type type)
        {
            if (view.IsMine)
            {
                Effect findedEffect = Effects.Find(n => n.GetType() == type);

                if (findedEffect != null)
                {
                    findedEffect.StopEffect();
                    Effects.Remove(findedEffect);
                    OnEffectRemoved?.Invoke(findedEffect);
                    Debug.Log(1223232323);
                }
            }
            else
                view.RPC("RPC_RemoveEffect", RpcTarget.All, type.Name);
        }

        private void HandleEffectEnd(Effect effect)
        {
            Effects.Remove(effect);
            OnEffectRemoved?.Invoke(effect);
        }
    }
}