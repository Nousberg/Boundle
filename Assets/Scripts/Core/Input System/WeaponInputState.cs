namespace Assets.Scripts.Core.Input_System
{
    public class WeaponInputState : InputState
    {
        public WeaponInputState()
        {
            ActiveBinds.Add(InputSystem.InputHandler.InputBind.MOUSERIGHT, true);
            ActiveBinds.Add(InputSystem.InputHandler.InputBind.MOUSELEFT, true);
            ActiveBinds.Add(InputSystem.InputHandler.InputBind.RELOADTOGGLE, true);

            BoolBinds.Add(InputSystem.InputHandler.InputBind.MOUSERIGHT, false);
            BoolBinds.Add(InputSystem.InputHandler.InputBind.MOUSELEFT, false);
        }
    }
}
