using UnityEngine;

namespace Assets.Scripts.Core.Input_System
{
    public class ToolgunInputState : InputState
    {
        public override void Init()
        {
            ActiveBinds.Add(InputSystem.InputHandler.InputBind.MOUSEWHEEL, true);

            VectorBinds.Add(InputSystem.InputHandler.InputBind.MOUSEWHEEL, Vector3.zero);
        }
    }
}
