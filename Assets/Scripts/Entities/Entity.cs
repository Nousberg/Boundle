using Assets.Scripts.Entities.Effects;
using System.Collections;
using System.Collections.Generic;
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

        public delegate void DamageHandler(float amount, Entity attacker, DamageType type);
        public event DamageHandler OnDamage;

        public float Blood { get; private set; }
        [field: SerializeField] public float Health { get; private set; }

        private EffectContainer effects => GetComponent<EffectContainer>();

        private void OnValidate()
        {
            if (!DamageSensors.Contains(DamageType.Generic))
                DamageSensors.Add(DamageType.Generic);
        }
        private void Start()
        {
            Blood = BaseHealth;
            Health = BaseHealth;

            effects.ApplyEffect(new Resistance(0f, 23f, true));
        }
        public void TakeDamage(float amount, Entity attacker, DamageType type)
        {
            if ((DamageSensors.Contains(type) && !Invulnerable) || type == DamageType.Generic)
            {
                effects.CalculateDamage(ref amount, attacker, type);

                StartCoroutine(CalculateBloodloss(amount));
                OnDamage?.Invoke(amount, attacker, type);
            }
        }
        private IEnumerator CalculateBloodloss(float initialBloodloss)
        {
            float maxDuration = 10f; // максимальная длительность кровотечения
            float minDuration = 1f; // минимальная длительность кровотечения
            float bloodlossFactor = Mathf.Clamp01(initialBloodloss / BaseHealth); // фактор интенсивности кровотечения

            // Обратная зависимость длительности от урона
            float totalDuration = Mathf.Lerp(maxDuration, minDuration, bloodlossFactor);

            float damagePerSecond = initialBloodloss / totalDuration; // увеличиваем скорость кровопотери
            float elapsedTime = 0f;

            // Начальный урон здоровью
            Health = Mathf.Clamp(Health - initialBloodloss / (Blood / BaseHealth), 0f, BaseHealth);

            while (elapsedTime < totalDuration)
            {
                elapsedTime += Time.deltaTime;
                float currentBloodloss = damagePerSecond * Time.deltaTime;

                Blood = Mathf.Clamp(Blood - currentBloodloss, 0f, BaseHealth);

                // Урон здоровью зависит от текущего уровня крови
                float healthDamage = currentBloodloss / Mathf.Max(0.1f, Blood / BaseHealth);
                Health = Mathf.Clamp(Health - healthDamage, 0f, BaseHealth);

                yield return null;
            }
        }
        public void Kill()
        {
            Health = 0f;
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