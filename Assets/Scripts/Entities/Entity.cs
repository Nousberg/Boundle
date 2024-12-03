using Assets.Scripts.Entities.Effects;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Entities.Liquids;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    [RequireComponent(typeof(PhysicsPropertiesContainer))]
    [RequireComponent(typeof(LiquidContainer))]
    public class Entity : MonoBehaviour, IDamageable
    {
        [field: Header("Entity properties")]
        [field: SerializeField] public EntityType MyType { get; private set; }
        [Min(0f)][SerializeField] private float criticalFallVelocity;

        [Header("Temperature")]
        [Range(0f, 1f)][SerializeField] private float temperatureResistance;
        [SerializeField] private float criticalMaxTemperature;
        [SerializeField] private float criticalMinTemperature;

        [Header("Liquid Properties")]
        [SerializeField] private float criticalMaxLiquidAmount;
        [SerializeField] private float criticalMinLiquidAmount;

        [Header("Harmful Liquid Properties")]
        [SerializeField] private float acidDamageFrequerency;

        [Header("Blood properties")]
        [Range(0f, 1f)][SerializeField] private float maxBloodlossPercentage;
        [Min(0f)][SerializeField] private float defaultBloodlossDuration;
        [SerializeField] private float normalBloodAmount;

        [Header("Regeneration")]
        [Range(0f, 1f)][SerializeField] private float maxRegenAmount;
        [SerializeField] private float regenerationRate;

        [field: Header("Health")]
        [field: Min(0f)][field: SerializeField] public float BaseHealth { get; private set; }
        [field: SerializeField] public bool Invulnerable { get; private set; }
        [field: SerializeField] public List<DamageType> DamageSensors { get; private set; } = new List<DamageType>();

        [field: Header("Food")]
        [field: Min(0f)][field: SerializeField] public float BaseFood { get; private set; }
        [Range(0f, 1f)][SerializeField] private float hungerDecreaseRate;
        [Range(0f, 1f)][SerializeField] private float waterDecreaseRate;
        [Range(0f, 1f)][SerializeField] private float damageFromLowHungerAmplifier;
        [Range(0f, 1f)][SerializeField] private float criticalHungerRate;

        public List<DamageEffect> Effects { get; private set; } = new List<DamageEffect>();

        public delegate void DamageHandler(ref float amount, DamageType type, Entity attacker);

        public event DamageHandler OnDamageTakenForEffects;
        public event Action<DamageEffect> OnEffectAdded;
        public event Action<DamageEffect> OnEffectRemoved;
        public event Action OnDeath;
        public event Action<float> OnDamageTaken;
        public event Action OnHealthChanged;
        public event Action OnFoodChanged;

        [field: SerializeField] public float Health { get; private set; }
        public float Food { get; private set; }
        public float Water { get; private set; }

        private PhysicsPropertiesContainer physicsData => GetComponent<PhysicsPropertiesContainer>();
        private LiquidContainer liquids => GetComponent<LiquidContainer>();

        private Liquid bloodRef;
        private float nextAcidDamage;
        private float nextBloodRegen;
        private float currentRegenAmount;
        private float nextTemperatureDamage;

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
            Health = BaseHealth;

            liquids.TryInject(LiquidContainer.LiquidType.Blood, normalBloodAmount);
            liquids.OnLiquidsChanged += GetBlood;

            GetBlood();
            StartCoroutine(StartHungerCycle());
        }
        private void Update()
        {
            HandleRegeneration();
            HandleTemperatureDamage();
            HandleLiquids();
        }
        private void GetBlood()
        {
            bloodRef = liquids.GetLiquids.Find(n => n.type == LiquidContainer.LiquidType.Blood);
        }
        private void HandleRegeneration()
        {
            if (Time.time >= nextBloodRegen)
            {
                if (bloodRef != null)
                {
                    nextBloodRegen = Time.time + 10f / regenerationRate;
                    currentRegenAmount = Health / BaseHealth;

                    liquids.TrySetLiquidAmount(liquids.GetLiquids.IndexOf(bloodRef), Mathf.Lerp(bloodRef.amount, normalBloodAmount, Mathf.Clamp(currentRegenAmount * Time.deltaTime, 0f, maxRegenAmount)));
                    Health = Mathf.Lerp(Health, BaseHealth, Mathf.Clamp(Mathf.Min(normalBloodAmount, bloodRef.amount) / Mathf.Max(normalBloodAmount, bloodRef.amount) * Time.deltaTime, 0f, maxRegenAmount));

                    OnHealthChanged?.Invoke();
                }
                else
                    Kill();
            }
        }
        private void HandleTemperatureDamage()
        {
            if (physicsData.Temperature > criticalMaxTemperature && Time.time >= nextTemperatureDamage)
            {
                nextTemperatureDamage = Time.time + criticalMaxTemperature / physicsData.Temperature;
                TakeDamage(physicsData.Temperature, this, DamageType.Temperature);
            }
            else if (physicsData.Temperature < criticalMinTemperature && Time.time >= nextTemperatureDamage)
            {
                nextTemperatureDamage = Time.time + physicsData.Temperature / criticalMinTemperature;
                TakeDamage(physicsData.Temperature, this, DamageType.Temperature);
            }
        }
        private void HandleLiquids()
        {
            foreach (var liquid in liquids.GetLiquids)
            {
                if (liquid.type == LiquidContainer.LiquidType.Acid && Time.time >= nextAcidDamage)
                {
                    nextAcidDamage = Time.time + 10f / acidDamageFrequerency;
                    TakeDamage(liquid.amount / Mathf.Clamp(liquids.Capacity / liquid.amount, 1f, float.PositiveInfinity), this, DamageType.Generic);
                }
            }
        }
        private IEnumerator ApplyBloodloss(float damage)
        {
            OnDamageTaken?.Invoke(damage);

            if (bloodRef == null || Health == 0f)
            {
                OnDeath?.Invoke();
                OnHealthChanged?.Invoke();
                yield break;
            }

            float dividedDamage = damage / 2f;
            float initialDamage = dividedDamage / Mathf.Clamp(Mathf.Min(physicsData.Temperature, physicsData.BaseTemperature) / Mathf.Max(physicsData.Temperature, physicsData.BaseTemperature) / (1f - temperatureResistance), 0.6f, 1f) / Mathf.Clamp(Mathf.Min(normalBloodAmount, bloodRef.amount) / Mathf.Max(normalBloodAmount, bloodRef.amount), 0.25f, 1f);

            Health = Mathf.Clamp(Health - initialDamage, 0f, BaseHealth);

            float damageAspect = damage / BaseHealth;
            float bloodlossDuration = defaultBloodlossDuration * damageAspect;
            float bloodloss = dividedDamage * Mathf.Clamp(bloodlossDuration / defaultBloodlossDuration, 0.1f, float.PositiveInfinity);

            float elapsedBloodlossDuration = 0f;
            float currentBloodloss = 0f;

            while (elapsedBloodlossDuration < bloodlossDuration)
            {
                elapsedBloodlossDuration += Time.deltaTime;
                currentBloodloss = bloodloss * (1f - elapsedBloodlossDuration / bloodlossDuration) * Time.deltaTime;

                liquids.TryPumpout(LiquidContainer.LiquidType.Blood, currentBloodloss);
                Health = Mathf.Clamp(Health - BaseHealth * (currentBloodloss / bloodRef.amount) * Time.deltaTime, 0f, BaseHealth);

                if (bloodRef == null || Health == 0f)
                {
                    OnDeath?.Invoke();
                    OnHealthChanged?.Invoke();
                    yield break;
                }

                OnHealthChanged?.Invoke();
                yield return null;
            }
        }

        public void TakeDamage(float amount, Entity attacker, DamageType type)
        {
            if ((DamageSensors.Contains(type) && !Invulnerable) || type == DamageType.Generic)
            {
                if (type != DamageType.Generic)
                    OnDamageTakenForEffects?.Invoke(ref amount, type, attacker);

                StartCoroutine(ApplyBloodloss(amount));
            }
        }
        public void RestoreFood(float foodAmount, float waterAmount = 0f)
        {
            Food = Mathf.Clamp(Food + foodAmount, Food, BaseFood);

            if (waterAmount > 0f)
                Water = Mathf.Clamp(Water + waterAmount, Water, BaseFood);

            OnFoodChanged?.Invoke();
        }
        public void ApplyEffect(DamageEffect effect)
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
            DamageEffect findedEffect = Effects.Find(n => n.GetType() == type);

            if (findedEffect != null)
            {
                findedEffect.StopEffect();
                Effects.Remove(findedEffect);
                OnEffectRemoved?.Invoke(findedEffect);
            }
        }
        public void Kill()
        {
            TakeDamage(Health, this, DamageType.Generic);
        }
        public void ToggleInvulnerability(bool state)
        {
            Invulnerable = state;
        }

        private void HandleEffectEnd(Effect effect)
        {
            Effects.Remove(Effects.Find(n => n.GetType() == effect.GetType()));
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
        Temperature,
        Gravity,
        Kenetic,
        Magic,
    }

    public enum EntityType : byte
    {
        Player,
        Default,
        Nextbot,
        Unmatched
    }
}