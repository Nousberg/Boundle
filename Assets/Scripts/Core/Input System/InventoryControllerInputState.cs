using UnityEngine;

namespace Assets.Scripts.Core.Input_System
{
    public class InventoryControllerInputState : InputState
    {
        public InventoryControllerInputState()
        {
            ActiveBinds.Add(InputSystem.InputHandler.InputBind.EQUIP, true);
            ActiveBinds.Add(InputSystem.InputHandler.InputBind.DROP, true);
            ActiveBinds.Add(InputSystem.InputHandler.InputBind.MOUSEWHEEL, true);

            VectorBinds.Add(InputSystem.InputHandler.InputBind.MOUSEWHEEL, Vector3.zero);
        }
    }
}
