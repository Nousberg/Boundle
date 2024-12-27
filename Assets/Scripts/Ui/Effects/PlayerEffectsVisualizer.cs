using Assets.Scripts.Effects;
using Assets.Scripts.Inventory;
using Assets.Scripts.Ui.Effects.Sciptables;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui.Effects
{
    [RequireComponent(typeof(EffectContainer))]
    public class PlayerEffectsVisualizer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<EffectUiData> effectsUiData = new List<EffectUiData>();
        [SerializeField] private Transform effectsParent;
        [SerializeField] private Transform itemEffectsParent;

        [Header("Properties")]
        [Min(0)][SerializeField] private int maxVisualizedItemEffects;
        [Min(0)][SerializeField] private int maxVisualizedEffects;

        private EffectContainer effectContainer => GetComponent<EffectContainer>();

        private List<GameObject> effectObjects = new List<GameObject>();
        private List<GameObject> itemEffectObjects = new List<GameObject>();
        private List<Effect> effectsOnObject = new List<Effect>();

        private void Start()
        {
            effectContainer.OnEffectAdded += AddEffect;
            effectContainer.OnEffectRemoved += RemoveEffect;
        }
        private void AddEffect(Effect effect)
        {
            effectsOnObject.Add(effect);
            VisualizeEffects();
        }
        private void RemoveEffect(Effect effect)
        {
            effectsOnObject.Remove(effect);
            VisualizeEffects();
        }
        private void VisualizeEffects()
        {
            foreach (var effect in effectObjects)
                Destroy(effect);

            effectObjects.Clear();

            foreach (var effect in effectsOnObject)
            {
                GameObject effectObject = new GameObject(effect.GetType().Name);
                Image effectImage = effectObject.AddComponent<Image>();
                effectImage.sprite = effectsUiData.Find(n => n.name == effect.GetType().Name).Icon;
                effectImage.preserveAspect = true;

                if (!(effect is ItemEffect) && effectObjects.Count <= maxVisualizedEffects)
                {
                    effectObject.transform.SetParent(effectsParent, false);
                    effectObjects.Add(effectObject);
                }
                else if ((effect is ItemEffect) && itemEffectObjects.Count <= maxVisualizedItemEffects)
                {
                    effectObject.transform.SetParent(itemEffectsParent, false);
                    itemEffectObjects.Add(effectObject);
                }
                else
                    Destroy(effectObject);
            }
        }
    }
}