using Assets.Scripts.Inventory;
using Assets.Scripts.Inventory.View;
using Assets.Scripts.Movement;
using Assets.Scripts.Ui.Inventory;
using UnityEngine;

namespace Assets.Scripts.Core.Input_System
{
    public class InputInitializer : MonoBehaviour
    {
        [SerializeField] private ItemSway sway;
        [SerializeField] private ToolgunDataController toolgun;
        [SerializeField] private PlayerInventoryController invController;
        [SerializeField] private PlayerMovementLogic movement;
        [SerializeField] private PlayerCameraLogic cam;

        public void Init(InputMachine machine)
        {
            MovementInputState movementState = new MovementInputState();
            ToolgunInputState toolgunState = new ToolgunInputState();
            InventoryControllerInputState inventoryState = new InventoryControllerInputState();

            sway.inputSource = movementState;
            movement.inputSource = movementState;
            cam.inputSource = movementState;
            invController.inputSource = inventoryState;
            toolgun.inputSource = toolgunState;

            machine.AddState(inventoryState);
            machine.AddState(toolgunState);
            machine.AddState(movementState);
            machine.Init();
        }
    }
}
