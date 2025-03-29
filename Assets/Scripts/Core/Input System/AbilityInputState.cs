namespace Assets.Scripts.Core.Input_System
{
    public class AbilityInputState : InputState
    {
        public AbilityInputState()
        {
            ActiveBinds.Add(InputSystem.InputHandler.InputBind.FLYTOGGLE, true);
        }
    }
}
