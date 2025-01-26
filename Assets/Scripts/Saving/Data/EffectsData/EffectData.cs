namespace Assets.Scripts.Saving
{
    public class EffectData
    {
        public string type;
        public int duration;
        public float amplifier;
        public bool infinite;

        public EffectData(int duration, float amplifier, bool infinite, string type)
        {
            this.infinite = infinite;
            this.duration = duration;
            this.amplifier = amplifier;
            this.type = type;
        }
    }
}
