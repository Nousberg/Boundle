using Assets.Scripts.Entities.Effects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.Entities
{
    public class Entity : MonoBehaviour
    {
        [field: Header("Health")]
        [Min(0f)][SerializeField] private float criticalFallVelocity;
        [field: Min(0f)][field: SerializeField] public float BaseHealth { get; private set; }
        [field: SerializeField] public bool Invulnerable { get; private set; }
        [field: SerializeField] public List<DamageType> DamageSensors { get; private set; } = new List<DamageType>();

        [field: Header("Food")]
        [field: Min(0f)][field: SerializeField] public float BaseFood { get; private set; }
        [Range(0f, 1f)][SerializeField] private float hungerDecreaseRate;
        [Range(0f, 1f)][SerializeField] private float waterDecreaseRate;
        [Range(0f, 1f)][SerializeField] private float damageFromLowHungerAmplifier;
        [Range(0f, 1f)][SerializeField] private float criticalHungerRate;

        public List<Effect> Effects { get; private set; } = new List<Effect>();

        public delegate void DamageHandler(ref float amount, DamageType type, Entity attacker);

        public event DamageHandler OnDamageTaken;
        public event Action OnEffectAdded;
        public event Action OnEffectRemoved;
        public event Action OnDeath;
        public event Action OnHealthChanged;
        public event Action OnFoodChanged;

        public float Blood { get; private set; }
        public float Health { get; private set; }
        public float Food { get; private set; }
        public float Water { get; private set; }

        private void OnValidate()
        {
            if (!DamageSensors.Contains(DamageType.Generic))
                DamageSensors.Add(DamageType.Generic);

            DamageSensors = DamageSensors.Distinct().ToList();
        }
        private void OnCollisionEnter(Collision collision)
        {
            float velocity = collision.relativeVelocity.magnitude;
            if (velocity >= criticalFallVelocity)
                TakeDamage(velocity, this, DamageType.Gravity);
        }
        private void Start()
        {
            Food = BaseFood;
            Water = BaseFood;
            Blood = BaseHealth;
            Health = BaseHealth;

            StartCoroutine(StartHungerCycle());
        }
        public void TakeDamage(float amount, Entity attacker, DamageType type)
        {
            if ((DamageSensors.Contains(type) && !Invulnerable) || type == DamageType.Generic)
            {
                OnDamageTaken?.Invoke(ref amount, type, attacker);
                StartCoroutine(CalculateBloodloss(amount));
            }
        }
        public void RestoreFood(float foodAmount, float waterAmount = 0f)
        {
            Food = Mathf.Clamp(Food + foodAmount, Food, BaseFood);

            if (waterAmount > 0f)
                Water = Mathf.Clamp(Water + waterAmount, Water, BaseFood);

            OnFoodChanged?.Invoke();
        }
        public void ApplyEffect(Effect effect)
        {
            if (!effect.isEnded)
            {
                effect.OnEffectEnded += HandleEffectEnd;
                Effects.Add(effect);
                OnEffectAdded?.Invoke();
            }
        }
        public void RemoveEffect(Type type)
        {
            Effect findedEffect = Effects.Find(n => n.GetType() == type);

            if (findedEffect != null)
            {
                findedEffect.StopEffect();
                Effects.Remove(findedEffect);
                OnEffectRemoved?.Invoke();
            }
        }
        public void Kill()
        {
            TakeDamage(Health, this, DamageType.Generic);
            OnHealthChanged?.Invoke();
        }
        public void ToggleInvulnerability(bool state)
        {
            Invulnerable = state;
        }

        private void HandleEffectEnd(Effect effect)
        {
            Effects.Remove(effect);
        }
        private IEnumerator CalculateBloodloss(float initialBloodloss)
        {
            float maxDuration = 10f;
            float minDuration = 1f;
            float bloodlossFactor = Mathf.Clamp01(initialBloodloss / BaseHealth);

            float totalDuration = Mathf.Lerp(maxDuration, minDuration, bloodlossFactor);

            float damagePerSecond = initialBloodloss / totalDuration;
            float elapsedTime = 0f;

            Health = Mathf.Clamp(Health - initialBloodloss / (Blood / BaseHealth), 0f, BaseHealth);

            while (elapsedTime < totalDuration)
            {
                elapsedTime += Time.deltaTime;
                float currentBloodloss = damagePerSecond * Time.deltaTime;

                Blood = Mathf.Clamp(Blood - currentBloodloss, 0f, BaseHealth);

                float healthDamage = currentBloodloss / Mathf.Max(0.1f, Blood / BaseHealth);
                Health = Mathf.Clamp(Health - healthDamage, 0f, BaseHealth);
                OnHealthChanged?.Invoke();

                if (Health == 0f)
                {
                    OnDeath?.Invoke();
                    break;
                }

                yield return null;
            }
        }
        private IEnumerator StartHungerCycle()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f / Mathf.Max(Food / BaseFood, 0.15f));

                if (Water > 0 || Food > 0)
                    OnFoodChanged?.Invoke();

                if (Food > 0f)
                    Food -= BaseFood * hungerDecreaseRate;

                if (Water > 0f)
                    Water -= BaseFood * waterDecreaseRate;

                if (Food < BaseFood * criticalHungerRate)
                    TakeDamage(Health * (1f - Food / BaseFood) * damageFromLowHungerAmplifier, this, DamageType.Generic);
            }
        }
    }
    public enum DamageType : byte
    {
        Generic,
        Gravity,
        Kenetic,
        Magic,
        Fire
    }
}