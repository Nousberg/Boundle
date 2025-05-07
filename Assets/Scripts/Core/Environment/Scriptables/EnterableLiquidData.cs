using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.Environment.Scriptables
{
    [CreateAssetMenu(fileName = "LiquidData", menuName = "ScriptableObjects/Triggerable/LiquidData")]
    public class EnterableLiquidData : ScriptableObject
    {
        [field: SerializeField] public List<EnterableLiquidEffect> AppliedEffects { get; private set; } = new List<EnterableLiquidEffect>();
        [field: SerializeField] public float dragModifier { get; private set; }
        [field: SerializeField] public float whileInTintIntensity { get; private set; }
        [field: SerializeField] public float objectWaveIntensity { get; private set; }
        [field: SerializeField] public float FoamInstantiateVelocity { get; private set; }
        [field: SerializeField] public int splashClip { get; private set; }
        [field: SerializeField] public int splashSource { get; private set; }
        [field: SerializeField] public Color whileInColor { get; private set; }
        [field: SerializeField] public GameObject AfterEnterFoam { get; private set; }

        public enum Effect : byte
        {
            Resistance,
            Damage
        }
    }
}
