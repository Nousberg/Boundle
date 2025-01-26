using Assets.Scripts.Movement;
using UnityEngine;

namespace Assets.Scripts.Entities.Abilities
{
    [RequireComponent(typeof(MovementController), typeof(Entity))]
    public class FlyAbility : Ability
    {
        [Header("Properties")]
        [Range(0f, 1f)][SerializeField] private float criticalDamage;

        private bool usable = true;

        private Entity player => GetComponent<Entity>();
        private MovementController playerMovement => GetComponent<MovementController>();

        public void Init() => player.OnDamageTaken += CheckDamageConditions;

        protected override void ToggleAbility()
        {
            if (usable)
                playerMovement.ToggleFly(true);
        }
        protected override void OnDeactivate()
        {
            playerMovement.ToggleFly(false);
        }

        public void ToggleUsability(bool state)
        {
            usable = state;

            if (!usable)
                Deactivate();
        }
        
        private void CheckDamageConditions(float amount)
        {
            if (amount > player.BaseHealth * criticalDamage)
                Deactivate();
        }
    }
}