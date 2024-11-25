namespace Assets.Scripts.Saving
{
    public class EffectData
    {
        public string type;
        public float duration;
        public float amplifier;
        public bool infinite;

        public EffectData(float duration, float amplifier, bool infinite, string type)
        {
            this.infinite = infinite;
            this.duration = duration;
            this.amplifier = amplifier;
            this.type = type;
        }
    }
}
