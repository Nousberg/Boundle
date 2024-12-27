using System;
using Assets.Scripts.Core.Environment.Scriptables;

namespace Assets.Scripts.Core.Environment
{
    [Serializable]
    public class EnterableLiquidEffect
    {
        public EnterableLiquidData.Effect effectType;
        public bool infinite;
        public float amplifier;
        public int duration;
    }
}
