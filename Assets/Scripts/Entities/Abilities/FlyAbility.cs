using Assets.Scripts.Movement;
using UnityEngine;

namespace Assets.Scripts.Entities.Abilities
{
    [RequireComponent(typeof(PlayerMovementLogic))]
    public class FlyAbility : Ability
    {
        private PlayerMovementLogic playerMovement => GetComponent<PlayerMovementLogic>();

        protected override void ToggleAbility()
        {
            playerMovement.ToggleFly(true);
        }
        protected override void OnDeactivate()
        {
            playerMovement.ToggleFly(false);
        }
    }
}