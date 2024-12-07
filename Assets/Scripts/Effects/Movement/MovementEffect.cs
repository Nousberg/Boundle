using Assets.Scripts.Movement;
using Assets.Scripts.Movement.Scriptables;

namespace Assets.Scripts.Effects.Movement
{
    public abstract class MovementEffect : Effect
    {
        protected readonly MovementController target;

        public MovementEffect(MovementController target, int duration, float amplifier, bool infinite = false) : base(duration, amplifier, infinite)
        {
            this.target = target;
            target.OnMove += ModifyMovement;
        }

        public abstract void ModifyMovement(MovementData data, ref float speed, ref float flySpeed, ref float runSpeedBoost, ref float jumpPower);
    }
}
