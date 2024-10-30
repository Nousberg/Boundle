using Assets.Scripts.Movement;
using UnityEngine;

namespace Assets.Scripts.Entities.Abilities
{
    [RequireComponent(typeof(PlayerMovementLogic))]
    public class FlyAbility : Ability
    {
        private bool usable = true;

        private PlayerMovementLogic playerMovement => GetComponent<PlayerMovementLogic>();

        protected override void ToggleAbility()
        {
            if (usable)
                playerMovement.ToggleFly(true);
            else
                Deactivate();
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
    }
}