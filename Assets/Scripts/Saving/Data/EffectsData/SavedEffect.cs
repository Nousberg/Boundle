using Assets.Scripts.Saving.Data;
using System;

namespace Assets.Scripts.Saving.EffectsData
{
    [Serializable]
    public class SavedEffect : SavedElement
    {
        public string type;
        public int duration;
        public float amplifier;
        public bool infinite;

        public SavedEffect(int duration, float amplifier, bool infinite, string type, int networkId) : base(networkId)
        {
            this.infinite = infinite;
            this.duration = duration;
            this.amplifier = amplifier;
            this.type = type;
        }
    }
}
