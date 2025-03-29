using UnityEngine;

namespace Assets.Scripts.Core.Input_System
{
    public class MovementInputState : InputState
    {
        public MovementInputState()
        {
            ActiveBinds.Add(InputSystem.InputHandler.InputBind.JUMP, true);
            ActiveBinds.Add(InputSystem.InputHandler.InputBind.RUNSTATE, true);
            ActiveBinds.Add(InputSystem.InputHandler.InputBind.LOOK, true);
            ActiveBinds.Add(InputSystem.InputHandler.InputBind.WASD, true);
            ActiveBinds.Add(InputSystem.InputHandler.InputBind.MOUSEWHEEL, true);

            VectorBinds.Add(InputSystem.InputHandler.InputBind.WASD, Vector3.zero);
            VectorBinds.Add(InputSystem.InputHandler.InputBind.LOOK, Vector3.zero);
            VectorBinds.Add(InputSystem.InputHandler.InputBind.MOUSEWHEEL, Vector3.zero);
        }
    }
}
