using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.Scripts.Effects
{
    public abstract class Effect
    {
        public readonly bool Infinite;

        public int RemainingLifetime { get; private set; }
        public int Duration { get; private set; }
        public float Amplifier { get; protected set; }
        public bool IsEnded { get; private set; }

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
        public void SetAmplifier(float value) => Amplifier = Math.Max(value, 0f);

        public void StopEffect()
        {
            tokenSource?.Cancel();
            IsEnded = true;
            OnEffectEnded?.Invoke(this);
        }

        public abstract void CombineEffects(Effect effect);

        protected void SetLifetime(int value)
        {
            if (value > -1 && value < int.MaxValue)
            {
                RemainingLifetime = value;

                if (value > Duration)
                    Duration = value;
            }
        }

        private async UniTask StartEffectLifeCycle(CancellationToken token)
        {
            int interval = 1000;

            RemainingLifetime = Duration;

            while (RemainingLifetime > 0)
            {
                if (token.IsCancellationRequested)
                    return;

                await UniTask.Delay(interval, cancellationToken: token);
                RemainingLifetime -= interval / 1000;
            }

            IsEnded = true;
            OnEffectEnded?.Invoke(this);
        }

    }
}