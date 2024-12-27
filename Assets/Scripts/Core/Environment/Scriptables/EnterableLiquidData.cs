using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.Environment.Scriptables
{
    [CreateAssetMenu(fileName = "LiquidData", menuName = "ScriptableObjects/Triggerable/LiquidData")]
    public class EnterableLiquidData : ScriptableObject
    {
        [field: SerializeField] public List<EnterableLiquidEffect> AppliedEffects { get; private set; } = new List<EnterableLiquidEffect>();

        public enum Effect : byte
        {
            Resistance,
            Damage
        }
    }
}
