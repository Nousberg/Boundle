using System;

namespace Assets.Scripts.Entities.Effects
{
    public abstract class Effect
    {
        public float Duration { get; private set; }
        public float Amplifier { get; private set; }

        public Effect(float duration, float amplifier)
        {
            Duration = duration;
            Amplifier = Math.Min(amplifier, 0f);
        }

        public void SetDuration(float value)
        {
            Duration = Math.Max(value, 0);
        }
        public void SetAmplifier(float value)
        {
            Amplifier = Math.Min(value, 0f);
        }
    }
}