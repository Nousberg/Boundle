using Assets.Scripts.Movement;
using Assets.Scripts.Movement.Scriptables;
using UnityEngine;

namespace Assets.Scripts.Effects.Movement
{
    public class Slowdown : MovementEffect
    {
        public Slowdown(MovementController target, int duration, float amplifier, bool infinite = false) : base(target, duration, amplifier, infinite)
        {

        }

        public override void ModifyMovement(MovementData data, ref float speed, ref float flySpeed, ref float runSpeedBoost, ref float jumpPower)
        {
            speed = data.WalkSpeed * (1f - Mathf.Clamp(Amplifier, 0f, 100f) / 100f);
        }
        public override void CombineEffects(Effect effect)
        {
            Amplifier += effect.Amplifier;
            SetLifetime(effect.Duration + RemainingLifetime);
        }
    }
}
