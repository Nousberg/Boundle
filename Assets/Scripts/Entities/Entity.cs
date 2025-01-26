using Assets.Scripts.Core.Interfaces;
using Assets.Scripts.Entities.Liquids;
using Assets.Scripts.Core.Environment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Assets.Scripts.Core;
using System.Linq;

namespace Assets.Scripts.Entities
{
    [RequireComponent(typeof(PhysicsPropertiesContainer))]
    [RequireComponent(typeof(LiquidContainer))]
    [RequireComponent(typeof(PhotonView))]
    public class Entity : MonoBehaviourPun, IPunObservable, IDamageable
    {
        [field: Header("Entity properties")]
        [field: SerializeField] public EntityType MyType { get; private set; }
        [Min(0f)][SerializeField] private float criticalFallVelocity;

        [Header("Temperature")]
        [Range(0f, 1f)][SerializeField] private float temperatureResistance;
        [SerializeField] private float normalTemperature;
        [SerializeField] private float criticalMaxTemperature;
        [SerializeField] private float criticalMinTemperature;

        [Header("Liquid Properties")]
        [SerializeField] private float criticalMaxLiquidAmount;
        [SerializeField] private float criticalMinLiquidAmount;

        [Header("Liquid Harm Properties")]
        [SerializeField] private float liquidCheckFrequency;

        [Header("Blood Properties")]
        [Range(0f, 1f)][SerializeField] private float maxBloodlossPercentage;
        [Min(0f)][SerializeField] private float maxBloodlossDuration;
        [SerializeField] private float normalBloodAmount;

        [Header("Regeneration")]
        [Range(0f, 1f)][SerializeField] private float maxRegenAmount;
        [SerializeField] private float regenerationRate;

        [field: Header("Health")]
        [field: Min(0f)][field: SerializeField] public float BaseHealth { get; private set; }
        [field: SerializeField] public bool Invulnerable { get; private set; }
        [field: SerializeField] public List<DamageData.DamageType> DamageSensors { get; private set; } = new List<DamageData.DamageType>();

        public delegate void DamageHandler(ref float amount, DamageData.DamageType type, Entity attacker);

        public event DamageHandler OnDamageTakenForEffects;
        public event Action<float> OnDamageTaken;
        public event Action OnDeath;
        public event Action OnHealthChanged;

        [field: SerializeField] public float Health { get; private set; }

        private PhysicsPropertiesContainer physicsData => GetComponent<PhysicsPropertiesContainer>();
        private LiquidContainer liquids => GetComponent<LiquidContainer>();
        private PhotonView view => GetComponent<PhotonView>();

        private Liquid bloodRef;
        private float nextLiqudCheck;
        private float nextRegen;
        private float currentRegenAmount;
        private float nextTemperatureDamage;

        public void Init()
        {
            Health = BaseHealth;

            liquids.OnLiquidsChanged += GetBloodRef;
            liquids.Inject(LiquidContainer.LiquidType.Blood, normalBloodAmount, view);
        }
        private void Update()
        {
            if (!photonView.IsMine || Dead())
                return;

            physicsData.CombineTemperature(normalTemperature);

            HandleRegeneration();
            HandleTemperatureDamage();
            HandleLiquids();
        }
        private void OnValidate()
        {
            criticalMaxTemperature = Mathf.Clamp(criticalMaxTemperature, criticalMinTemperature, float.PositiveInfinity);
            criticalMinTemperature = Mathf.Clamp(criticalMinTemperature, float.NegativeInfinity, criticalMaxTemperature);
            normalTemperature = Mathf.Clamp(normalTemperature, criticalMinTemperature, criticalMaxTemperature);
        }
        private void OnCollisionEnter(Collision collision)
        {
            float velocity = collision.relativeVelocity.magnitude;

            if (velocity >= criticalFallVelocity)
                TakeDamage(velocity, this, DamageData.DamageType.Gravity);
        }
        private void GetBloodRef()
        {
            bloodRef = liquids.GetLiquids.Find(n => n.type == LiquidContainer.LiquidType.Blood);
        }
        private void HandleRegeneration()
        {
            if (Time.time >= nextRegen)
            {
                if (bloodRef != null)
                {
                    nextRegen = Time.time + 10f / regenerationRate;
                    currentRegenAmount = Health / BaseHealth;

                    Heal(BaseHealth * currentRegenAmount * Mathf.Clamp(Mathf.Min(normalBloodAmount, bloodRef.amount) / Mathf.Max(normalBloodAmount, bloodRef.amount), 0f, maxRegenAmount));

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
                TakeDamage((physicsData.Temperature - criticalMaxTemperature) * (1f - temperatureResistance), this, DamageData.DamageType.Magic);
            }
            else if (physicsData.Temperature < criticalMinTemperature && Time.time >= nextTemperatureDamage)
            {
                nextTemperatureDamage = Time.time + physicsData.Temperature / criticalMinTemperature;
                TakeDamage((criticalMinTemperature - physicsData.Temperature) * (1f - temperatureResistance), this, DamageData.DamageType.Magic);
            }
        }
        private void HandleLiquids()
        {
            if (Time.time >= nextLiqudCheck)
            {
                nextLiqudCheck = Time.time + 10f / liquidCheckFrequency;

                foreach (var liquid in liquids.GetLiquids)
                {
                    switch (liquid.type)
                    {
                        case LiquidContainer.LiquidType.Acid:
                            TakeDamage(liquid.amount / Mathf.Clamp(bloodRef.amount / liquid.amount, 1f, float.PositiveInfinity), this, DamageData.DamageType.Magic);
                            break;
                        case LiquidContainer.LiquidType.Mending:
                            Heal(liquid.amount);
                            break;
                    }
                }
            }
        }
        private bool Dead()
        {
            if (Health <= 0f || bloodRef == null)
            {
                OnDeath?.Invoke();
                OnHealthChanged?.Invoke();
                return true;
            }
            return false;
        }
        private IEnumerator ApplyBloodloss(float damage)
        {
            if (Dead())
                yield break;

            float dividedDamage = damage / 2f;

            float clampedDamageInfluenceFactors = Mathf.Clamp(
                Mathf.Min(physicsData.Temperature, normalTemperature) /
                Mathf.Max(physicsData.Temperature, normalTemperature)

                *

                (Mathf.Min(bloodRef.amount, normalBloodAmount) /
                Mathf.Max(bloodRef.amount, normalBloodAmount)), 0.1f, 1f);

            Health = Mathf.Max(Health - dividedDamage / (clampedDamageInfluenceFactors), 0f);

            float bloodloss = dividedDamage / BaseHealth;
            float clampedBloodloss = Mathf.Min(bloodloss, 1f);
            float duration = maxBloodlossDuration * clampedBloodloss;
            float elapsedTime = 0f;

            currentRegenAmount = currentRegenAmount * (1f - clampedBloodloss);

            while (duration > elapsedTime)
            {
                if (Dead())
                    yield break;

                elapsedTime += Time.deltaTime;

                float timeFactor = Mathf.Min(bloodloss, maxBloodlossPercentage) * (1f - elapsedTime / duration) * Time.deltaTime;

                Health = Mathf.Max(Health - damage * timeFactor, 0f);
                liquids.Pumpout(LiquidContainer.LiquidType.Blood, normalBloodAmount * timeFactor);

                yield return null;
            }
        }
        [PunRPC]
        private void RPC_TakeDamageToOwner(float amount, int attackerViewId, int type)
        {
            if (view.IsMine)
            {
                List<Entity> es = FindObjectsOfType<Entity>().ToList();
                Entity attacker = null;

                foreach (Entity e in es)
                    if (e.TryGetComponent<PhotonView>(out var v))
                        if (v.ViewID == attackerViewId)
                            attacker = e;

                if (attacker != null)
                    TakeDamage(amount, attacker, (DamageData.DamageType)type);
            }
        }
        [PunRPC]
        private void RPC_HealOwner(float amount)
        {
            if (view.IsMine)
                Heal(amount);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(Convert.ToDouble(Health));
            }
            else
            {
                Health = Convert.ToSingle(stream.ReceiveNext());
            }
        }
        public void Heal(float amount)
        {
            if (view.IsMine)
            {
                Health = Mathf.Clamp(Health + amount, 0f, BaseHealth);
                liquids.SetLiquidAmount(liquids.GetLiquids.IndexOf(bloodRef), Mathf.Lerp(bloodRef.amount, normalBloodAmount, Mathf.Clamp(Health / BaseHealth, 0f, maxRegenAmount)));
            }
            else
                view.RPC("RPC_HealOwner", RpcTarget.All, amount);
        }
        public void TakeDamage(float amount, Entity attacker, DamageData.DamageType type)
        {
            if ((DamageSensors.Contains(type) && !Invulnerable) || DataContainer.DamageProperties[type].IgnoreDefence || DataContainer.DamageProperties[type].InvulnerabilityMitigation > 0f)
            {
                if (Invulnerable && DataContainer.DamageProperties[type].InvulnerabilityMitigation > 0f)
                    amount *= DataContainer.DamageProperties[type].InvulnerabilityMitigation;

                if (view.IsMine)
                {
                    if (!DataContainer.DamageProperties[type].IgnoreDefence)
                        OnDamageTakenForEffects?.Invoke(ref amount, type, attacker);

                    OnDamageTaken?.Invoke(amount);

                    StartCoroutine(ApplyBloodloss(amount));
                }
                else
                    view.RPC("RPC_TakeDamageToOwner", RpcTarget.All, amount, attacker.GetComponent<PhotonView>().ViewID, (int)type);
            }
        }
        public void Kill()
        {
            TakeDamage(Health, this, DamageData.DamageType.Generic);
        }
        public void ToggleInvulnerability(bool state) => Invulnerable = state;
    }

    public enum EntityType : byte
    {
        Player,
        Enemy,
        Nextbot,
        Unmatched
    }
}