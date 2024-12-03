using System;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.Scripts.Entities.Effects
{
    public abstract class Effect
    {
        public readonly bool Infinite;

        public int Duration { get; private set; }
        public float Amplifier { get; protected set; }
        public bool isEnded { get; private set; }

        public event Action<Effect> OnEffectEnded;

        private CancellationTokenSource tokenSource;

        public Effect(int duration, float amplifier, bool infinite = false)
        {
            Amplifier = Math.Max(amplifier, 1f);
            Duration = duration;
            Infinite = infinite;

            if (!infinite)
            {
                tokenSource = new CancellationTokenSource();
                _ = StartEffectLifeCycle(tokenSource.Token);
            }
        }
        public void SetAmplifier(float value)
        {
            Amplifier = Math.Max(value, 0f);
        }
        public void SetDuration(int value)
        {
            if (value < 0)
                return;

            Duration = value;
            tokenSource.Cancel();
            _ = StartEffectLifeCycle(tokenSource.Token);
        }
        public void StopEffect()
        {
            tokenSource?.Cancel();
            isEnded = true;
            OnEffectEnded?.Invoke(this);
        }

        public abstract void CombineEffects(Effect effect);

        private async Task StartEffectLifeCycle(CancellationToken token)
        {
            await Task.Delay(Duration, token);
            isEnded = true;
            OnEffectEnded?.Invoke(this);
        }
    }
}