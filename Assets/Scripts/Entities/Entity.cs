using Assets.Scripts.Entities.Effects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.Entities
{
    [RequireComponent(typeof(EffectContainer))]
    public class Entity : MonoBehaviour
    {
        [field: Header("Health")]
        [field: Min(0f)][field: SerializeField] public float BaseHealth { get; private set; }
        [field: SerializeField] public bool Invulnerable { get; private set; }
        [field: SerializeField] public List<DamageType> DamageSensors { get; private set; } = new List<DamageType>();

        [field: Header("Food")]
        [Min(0f)][SerializeField] private float foodDecreaseRate;
        [Range(0f, 1f)][SerializeField] private float damageFromLowWaterRateAmplifier;
        [Range(0f, 1f)][SerializeField] private float criticalWaterRate;
        [field: Min(0f)][field: SerializeField] public float BaseFood { get; private set; }

        public delegate void DamageHandler(float amount, Entity attacker, DamageType type);
        public event DamageHandler OnDamage;

        public event Action OnHealthChanged;
        public event Action OnFoodChanged;

        public float Blood { get; private set; }
        public float Health { get; private set; }
        public float Food { get; private set; }
        public float Water { get; private set; }

        private EffectContainer effects => GetComponent<EffectContainer>();

        private void OnValidate()
        {
            if (!DamageSensors.Contains(DamageType.Generic))
                DamageSensors.Add(DamageType.Generic);

            DamageSensors = DamageSensors.Distinct().ToList();
        }
        private void Start()
        {
            Food = BaseFood;
            Water = BaseFood;
            Blood = BaseHealth;
            Health = BaseHealth;

            StartCoroutine(StartHungerCycle());
            effects.ApplyEffect(new Resistance(0f, 7f, true));
        }
        public void TakeDamage(float amount, Entity attacker, DamageType type)
        {
            if ((DamageSensors.Contains(type) && !Invulnerable) || type == DamageType.Generic)
            {
                effects.CalculateDamage(ref amount, attacker, type);

                StartCoroutine(CalculateBloodloss(amount));
                OnDamage?.Invoke(amount, attacker, type);
                OnHealthChanged?.Invoke();
            }
        }
        public void RestoreFood(float foodAmount, float waterAmount = 0f)
        {
            Food = Mathf.Clamp(Food + foodAmount, Food, BaseFood);

            if (waterAmount > 0f)
                Water = Mathf.Clamp(Water + waterAmount, Water, BaseFood);

            OnFoodChanged?.Invoke();
        }
        private IEnumerator CalculateBloodloss(float initialBloodloss)
        {
            float maxDuration = 10f;
            float minDuration = 1f;
            float bloodlossFactor = Mathf.Clamp01(initialBloodloss / BaseHealth);

            float totalDuration = Mathf.Lerp(maxDuration, minDuration, bloodlossFactor);

            float damagePerSecond = initialBloodloss / totalDuration;
            float elapsedTime = 0f;

            float healthDamage = 0f;
            float currentBloodloss = 0f;

            Health = Mathf.Clamp(Health - initialBloodloss / (Blood / BaseHealth), 0f, BaseHealth);

            while (elapsedTime < totalDuration)
            {
                elapsedTime += Time.deltaTime;
                healthDamage = damagePerSecond * Time.deltaTime;

                Blood = Mathf.Clamp(Blood - currentBloodloss, 0f, BaseHealth);

                healthDamage = currentBloodloss / Mathf.Max(0.1f, Blood / BaseHealth);
                Health = Mathf.Clamp(Health - healthDamage, 0f, BaseHealth);

                yield return null;
            }
        }
        private IEnumerator StartHungerCycle()
        {
            while (true)
            {
                yield return new WaitForSeconds(foodDecreaseRate / BaseFood);

                if (Food > 0)
                    Food--;

                if (Water > 0)
                    Water--;

                if (Water < BaseFood * criticalWaterRate)
                    TakeDamage(Health * (1f - Water / BaseFood) * damageFromLowWaterRateAmplifier, this, DamageType.Generic);

                OnFoodChanged?.Invoke();
            }
        }

        public void Kill()
        {
            Health = 0f;
            OnHealthChanged?.Invoke();
        }
        public void ToggleInvulnerability(bool state)
        {
            Invulnerable = state;
        }
    }
    public enum DamageType : byte
    {
        Generic,
        Fall,
        Kenetic,
        Magic,
        Fire
    }
}