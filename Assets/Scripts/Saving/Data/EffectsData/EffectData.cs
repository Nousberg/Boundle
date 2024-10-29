namespace Assets.Scripts.Saving
{
    public class EffectData
    {
        public float duration;
        public float amplifier;
        public bool infinite;
        public string type;

        public EffectData(float duration, float amplifier, bool infinite, string type)
        {
            this.infinite = infinite;
            this.duration = duration;
            this.amplifier = amplifier;
            this.type = type;
        }
    }
}
