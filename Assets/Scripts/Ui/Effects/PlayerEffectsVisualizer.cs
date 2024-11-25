using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Effects;
using Assets.Scripts.Inventory;
using Assets.Scripts.Movement;
using Assets.Scripts.Ui.Effects.Sciptables;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui.Effects
{
    public class PlayerEffectsVisualizer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<EffectUiData> effectsUiData = new List<EffectUiData>();
        [SerializeField] private Entity player;
        [SerializeField] private InventoryDataController inventory;
        [SerializeField] private MovementController movement;
        [SerializeField] private Transform effectsParent;

        private List<GameObject> effectObjects = new List<GameObject>();
        private List<Effect> effectsOnObject = new List<Effect>();

        private void Start()
        {
            player.OnEffectAdded += AddEffect;
            player.OnEffectRemoved += RemoveEffect;

            movement.OnEffectAdded += AddEffect;
            movement.OnEffectRemoved += RemoveEffect;

            inventory.OnItemAdded += HandleInventoryChange;
            inventory.OnItemAdded += HandleInventoryChange;
            inventory.OnItemRemoved += HandleInventoryChange;
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
        private void HandleInventoryChange()
        {
            foreach (var item in inventory.AllInGameItems)
            {
                item.OnEffectAdded += AddEffect;
                item.OnEffectRemoved += RemoveEffect;
            }
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

                effectObject.transform.SetParent(effectsParent, false);

                effectObjects.Add(effectObject);
            }
        }
    }
}