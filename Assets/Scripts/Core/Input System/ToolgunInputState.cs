using UnityEngine;

namespace Assets.Scripts.Core.Input_System
{
    public class ToolgunInputState : InputState
    {
        public ToolgunInputState()
        {
            ActiveBinds.Add(InputSystem.InputHandler.InputBind.MOUSEWHEEL, true);
            ActiveBinds.Add(InputSystem.InputHandler.InputBind.MOUSEMID, true);
            ActiveBinds.Add(InputSystem.InputHandler.InputBind.MOUSELEFT, true);
            ActiveBinds.Add(InputSystem.InputHandler.InputBind.MOUSERIGHT, true);

            VectorBinds.Add(InputSystem.InputHandler.InputBind.MOUSEWHEEL, Vector3.zero);

            BoolBinds.Add(InputSystem.InputHandler.InputBind.MOUSELEFT, false);
        }
    }
}
