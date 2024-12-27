using Assets.Scripts.Core.Environment.Scriptables;
using Assets.Scripts.Effects;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Liquids;
using UnityEngine;

namespace Assets.Scripts.Core.Environment
{
    [RequireComponent(typeof(LiquidContainer))]
    public class EnterableLiquid : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EnterableLiquidData data;

        [Header("Properties")]
        [Range(0f, 1f)][SerializeField] private float liquidTransferPercentage;

        private LiquidContainer liquids => GetComponent<LiquidContainer>();

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<EffectContainer>(out var container) && other.TryGetComponent<Entity>(out var entity))
            {
                foreach (var effect in data.AppliedEffects)
                {
                    switch (effect.effectType)
                    {
                        case EnterableLiquidData.Effect.Resistance:
                            container.ApplyEffect(new Resistance(entity, effect.duration, effect.amplifier, effect.infinite));
                            break;
                    }
                }
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent<LiquidContainer>(out var container))
                liquids.Transfer(container, liquids.Weight * liquidTransferPercentage * Time.deltaTime);
        }
    }
}