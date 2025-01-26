using Assets.Scripts.Core.InputSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.Input_System
{
    [RequireComponent(typeof(InputHandler))]
    public class InputMachine : MonoBehaviour
    {
        public bool blockInput;
        public List<InputState> GetStates => states;

        private InputHandler input => GetComponent<InputHandler>();

        private List<InputState> states = new List<InputState>();

        public void Init()
        {
            foreach (var state in states)
            {
                if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.JUMP) && state.ActiveBinds[InputHandler.InputBind.JUMP])
                    input.Binds[InputHandler.InputBind.JUMP].performed += ctx 
                        => state.ToggleInputEvent(InputHandler.InputBind.JUMP);

                if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.EQUIP) && state.ActiveBinds[InputHandler.InputBind.EQUIP])
                    input.Binds[InputHandler.InputBind.EQUIP].performed += ctx
                        => state.ToggleInputEvent(InputHandler.InputBind.EQUIP);

                if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.DROP) && state.ActiveBinds[InputHandler.InputBind.DROP])
                    input.Binds[InputHandler.InputBind.DROP].performed += ctx
                        => state.ToggleInputEvent(InputHandler.InputBind.DROP);

                if (state.BoolBinds.ContainsKey(InputHandler.InputBind.RUNSTATE) && state.ActiveBinds[InputHandler.InputBind.RUNSTATE])
                    state.BoolBinds[InputHandler.InputBind.RUNSTATE] = () => input.BoolBinds[InputHandler.InputBind.RUNSTATE]();
            }
        }

        private void Update()
        {
            if (!blockInput)
                foreach (var state in states)
                {
                    if (state.VectorBinds.ContainsKey(InputHandler.InputBind.WASD))
                    {
                        if (state.ActiveBinds[InputHandler.InputBind.WASD])
                            state.VectorBinds[InputHandler.InputBind.WASD] = input.VectorBinds[InputHandler.InputBind.WASD];
                        else
                            state.VectorBinds[InputHandler.InputBind.WASD] = Vector2.zero;
                    }

                    if (state.VectorBinds.ContainsKey(InputHandler.InputBind.LOOK))
                    {
                        if (state.ActiveBinds[InputHandler.InputBind.LOOK])
                            state.VectorBinds[InputHandler.InputBind.LOOK] = input.VectorBinds[InputHandler.InputBind.LOOK];
                        else
                            state.VectorBinds[InputHandler.InputBind.LOOK] = Vector2.zero;
                    }

                    if (state.VectorBinds.ContainsKey(InputHandler.InputBind.MOUSEWHEEL))
                    {
                        if (state.ActiveBinds[InputHandler.InputBind.MOUSEWHEEL])
                            state.VectorBinds[InputHandler.InputBind.MOUSEWHEEL] = input.VectorBinds[InputHandler.InputBind.MOUSEWHEEL];
                        else
                            state.VectorBinds[InputHandler.InputBind.MOUSEWHEEL] = Vector2.zero;
                    }
                }
        }

        public void AddState(InputState state)
        {
            if (!states.Contains(state))
            {
                state.Init();
                states.Add(state);
            }
        }
        public void SwitchBindState(InputHandler.InputBind bind, int stateIndex)
        {
            if (stateIndex < 0 || stateIndex >= states.Count)
                return;

            foreach (var state in states)
                state.ActiveBinds[bind] = false;

            states[stateIndex].ActiveBinds[bind] = true;
        }
        public void SetBindActiveForEveryState(InputHandler.InputBind bind, bool active)
        {
            foreach (var state in states)
                if (state.ActiveBinds.ContainsKey(bind))
                    state.ActiveBinds[bind] = active;
        }
    }
}
