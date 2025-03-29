using Assets.Scripts.Movement;
using UnityEngine;

namespace Assets.Scripts.Entities.Abilities
{
    [RequireComponent(typeof(MovementController), typeof(Entity))]
    public class FlyAbility : Ability
    {
        [Header("Properties")]
        [Range(0f, 1f)][SerializeField] private float criticalDamage;

        private bool usable;

        private Entity player => GetComponent<Entity>();
        private MovementController playerMovement => GetComponent<MovementController>();

        protected override void OnInit()
        {
            player.OnDamageTaken += CheckDamageConditions;
        }

        protected override void ToggleAbility()
        {
            if (gameObject == null)
                return;

            if (usable)
                playerMovement.ToggleFly(true);
        }

        protected override void OnDeactivate()
        {
            if (gameObject == null)
                return;

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

        private void OnDestroy()
        {
            if (player != null)
                player.OnDamageTaken -= CheckDamageConditions;
        }
    }
}
