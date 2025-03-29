using Assets.Scripts.Core.InputSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Core.Input_System
{
    [RequireComponent(typeof(InputHandler))]
    public class InputMachine : MonoBehaviour
    {
        public List<InputState> GetStates => new List<InputState>(states);

        private InputHandler input => GetComponent<InputHandler>();

        private List<InputState> states = new List<InputState>();
        private bool blockInput;

        public void Init() => BlockInput(false);

        private void Update()
        {
            if (!blockInput)
                foreach (var state in states)
                {
                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.RELOADTOGGLE))
                        state.BoolBinds[InputHandler.InputBind.RELOADTOGGLE] = input.BoolBinds[InputHandler.InputBind.RELOADTOGGLE]();

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.MOUSEMID))
                        state.BoolBinds[InputHandler.InputBind.MOUSEMID] = input.BoolBinds[InputHandler.InputBind.MOUSEMID]();

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.RUNSTATE))
                        state.BoolBinds[InputHandler.InputBind.RUNSTATE] = input.BoolBinds[InputHandler.InputBind.RUNSTATE]();

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.MOUSELEFT))
                        state.BoolBinds[InputHandler.InputBind.MOUSELEFT] = input.BoolBinds[InputHandler.InputBind.MOUSELEFT]();

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.MOUSERIGHT))
                        state.BoolBinds[InputHandler.InputBind.MOUSERIGHT] = input.BoolBinds[InputHandler.InputBind.MOUSERIGHT]();

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.FLYTOGGLE))
                        state.BoolBinds[InputHandler.InputBind.FLYTOGGLE] = input.BoolBinds[InputHandler.InputBind.FLYTOGGLE]();


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

        public void BlockInput(bool value)
        {
            blockInput = value;

            if (value)
            {
                foreach (var state in states)
                {
                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.RELOADTOGGLE) && state.ActiveBinds[InputHandler.InputBind.RELOADTOGGLE])
                        input.Binds[InputHandler.InputBind.RELOADTOGGLE].performed -= OnReloadTogglePerfomed;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.MOUSEMID) && state.ActiveBinds[InputHandler.InputBind.MOUSEMID])
                        input.Binds[InputHandler.InputBind.MOUSEMID].performed -= OnMouseMidPerfomed;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.MOUSELEFT) && state.ActiveBinds[InputHandler.InputBind.MOUSELEFT])
                        input.Binds[InputHandler.InputBind.MOUSELEFT].performed -= OnMouseLeftPerformed;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.MOUSERIGHT) && state.ActiveBinds[InputHandler.InputBind.MOUSERIGHT])
                        input.Binds[InputHandler.InputBind.MOUSERIGHT].performed -= OnMouseRightPerformed;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.FLYTOGGLE) && state.ActiveBinds[InputHandler.InputBind.FLYTOGGLE])
                        input.Binds[InputHandler.InputBind.FLYTOGGLE].performed -= OnFlyTogglePerformed;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.JUMP) && state.ActiveBinds[InputHandler.InputBind.JUMP])
                        input.Binds[InputHandler.InputBind.JUMP].performed -= OnJumpPerformed;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.EQUIP) && state.ActiveBinds[InputHandler.InputBind.EQUIP])
                        input.Binds[InputHandler.InputBind.EQUIP].performed -= OnEquipPerformed;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.DROP) && state.ActiveBinds[InputHandler.InputBind.DROP])
                        input.Binds[InputHandler.InputBind.DROP].performed -= OnDropPerformed;


                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.RELOADTOGGLE) && state.ActiveBinds[InputHandler.InputBind.RELOADTOGGLE])
                        input.Binds[InputHandler.InputBind.RELOADTOGGLE].canceled -= OnReloadToggleCanceled;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.MOUSEMID) && state.ActiveBinds[InputHandler.InputBind.MOUSEMID])
                        input.Binds[InputHandler.InputBind.MOUSEMID].canceled -= OnMouseMidCanceled;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.MOUSELEFT) && state.ActiveBinds[InputHandler.InputBind.MOUSELEFT])
                        input.Binds[InputHandler.InputBind.MOUSELEFT].canceled -= OnMouseLeftCanceled;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.MOUSERIGHT) && state.ActiveBinds[InputHandler.InputBind.MOUSERIGHT])
                        input.Binds[InputHandler.InputBind.MOUSERIGHT].canceled -= OnMouseRightCanceled;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.FLYTOGGLE) && state.ActiveBinds[InputHandler.InputBind.FLYTOGGLE])
                        input.Binds[InputHandler.InputBind.FLYTOGGLE].canceled -= OnFlyToggleCanceled;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.JUMP) && state.ActiveBinds[InputHandler.InputBind.JUMP])
                        input.Binds[InputHandler.InputBind.JUMP].canceled -= OnJumpCanceled;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.EQUIP) && state.ActiveBinds[InputHandler.InputBind.EQUIP])
                        input.Binds[InputHandler.InputBind.EQUIP].canceled -= OnEquipCanceled;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.DROP) && state.ActiveBinds[InputHandler.InputBind.DROP])
                        input.Binds[InputHandler.InputBind.DROP].canceled -= OnDropCanceled;


                    if (state.VectorBinds.ContainsKey(InputHandler.InputBind.WASD))
                        state.VectorBinds[InputHandler.InputBind.WASD] = Vector2.zero;

                    if (state.VectorBinds.ContainsKey(InputHandler.InputBind.LOOK))
                        state.VectorBinds[InputHandler.InputBind.LOOK] = Vector2.zero;

                    if (state.VectorBinds.ContainsKey(InputHandler.InputBind.MOUSEWHEEL))
                        state.VectorBinds[InputHandler.InputBind.MOUSEWHEEL] = Vector2.zero;

                    if (state.BoolBinds.ContainsKey(InputHandler.InputBind.MOUSELEFT))
                        state.BoolBinds[InputHandler.InputBind.MOUSELEFT] = false;
                }
            }
            else
            {
                foreach (var state in states)
                {
                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.RELOADTOGGLE) && state.ActiveBinds[InputHandler.InputBind.RELOADTOGGLE])
                        input.Binds[InputHandler.InputBind.RELOADTOGGLE].performed += OnReloadTogglePerfomed;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.MOUSEMID) && state.ActiveBinds[InputHandler.InputBind.MOUSEMID])
                        input.Binds[InputHandler.InputBind.MOUSEMID].performed += OnMouseMidPerfomed;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.MOUSELEFT) && state.ActiveBinds[InputHandler.InputBind.MOUSELEFT])
                        input.Binds[InputHandler.InputBind.MOUSELEFT].performed += OnMouseLeftPerformed;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.MOUSERIGHT) && state.ActiveBinds[InputHandler.InputBind.MOUSERIGHT])
                        input.Binds[InputHandler.InputBind.MOUSERIGHT].performed += OnMouseRightPerformed;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.FLYTOGGLE) && state.ActiveBinds[InputHandler.InputBind.FLYTOGGLE])
                        input.Binds[InputHandler.InputBind.FLYTOGGLE].performed += OnFlyTogglePerformed;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.JUMP) && state.ActiveBinds[InputHandler.InputBind.JUMP])
                        input.Binds[InputHandler.InputBind.JUMP].performed += OnJumpPerformed;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.EQUIP) && state.ActiveBinds[InputHandler.InputBind.EQUIP])
                        input.Binds[InputHandler.InputBind.EQUIP].performed += OnEquipPerformed;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.DROP) && state.ActiveBinds[InputHandler.InputBind.DROP])
                        input.Binds[InputHandler.InputBind.DROP].performed += OnDropPerformed;


                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.RELOADTOGGLE) && state.ActiveBinds[InputHandler.InputBind.RELOADTOGGLE])
                        input.Binds[InputHandler.InputBind.RELOADTOGGLE].canceled += OnReloadToggleCanceled;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.MOUSEMID) && state.ActiveBinds[InputHandler.InputBind.MOUSEMID])
                        input.Binds[InputHandler.InputBind.MOUSEMID].canceled += OnMouseMidCanceled;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.MOUSELEFT) && state.ActiveBinds[InputHandler.InputBind.MOUSELEFT])
                        input.Binds[InputHandler.InputBind.MOUSELEFT].canceled += OnMouseLeftCanceled;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.MOUSERIGHT) && state.ActiveBinds[InputHandler.InputBind.MOUSERIGHT])
                        input.Binds[InputHandler.InputBind.MOUSERIGHT].canceled += OnMouseRightCanceled;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.FLYTOGGLE) && state.ActiveBinds[InputHandler.InputBind.FLYTOGGLE])
                        input.Binds[InputHandler.InputBind.FLYTOGGLE].canceled += OnFlyToggleCanceled;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.JUMP) && state.ActiveBinds[InputHandler.InputBind.JUMP])
                        input.Binds[InputHandler.InputBind.JUMP].canceled += OnJumpCanceled;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.EQUIP) && state.ActiveBinds[InputHandler.InputBind.EQUIP])
                        input.Binds[InputHandler.InputBind.EQUIP].canceled += OnEquipCanceled;

                    if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.DROP) && state.ActiveBinds[InputHandler.InputBind.DROP])
                        input.Binds[InputHandler.InputBind.DROP].canceled += OnDropCanceled;
                }
            }
        }

        private void OnReloadToggleCanceled(InputAction.CallbackContext ctx)
        {
            foreach (var state in states)
            {
                if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.RELOADTOGGLE) && state.ActiveBinds[InputHandler.InputBind.RELOADTOGGLE])
                    state.ToggleInputCancelEvent(InputHandler.InputBind.RELOADTOGGLE);
            }
        }
        private void OnMouseMidCanceled(InputAction.CallbackContext ctx)
        {
            foreach (var state in states)
            {
                if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.MOUSEMID) && state.ActiveBinds[InputHandler.InputBind.MOUSEMID])
                    state.ToggleInputCancelEvent(InputHandler.InputBind.MOUSEMID);
            }
        }
        private void OnMouseLeftCanceled(InputAction.CallbackContext ctx)
        {
            foreach (var state in states)
            {
                if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.MOUSELEFT) && state.ActiveBinds[InputHandler.InputBind.MOUSELEFT])
                    state.ToggleInputCancelEvent(InputHandler.InputBind.MOUSELEFT);
            }
        }

        private void OnMouseRightCanceled(InputAction.CallbackContext ctx)
        {
            foreach (var state in states)
            {
                if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.MOUSERIGHT) && state.ActiveBinds[InputHandler.InputBind.MOUSERIGHT])
                    state.ToggleInputCancelEvent(InputHandler.InputBind.MOUSERIGHT);
            }
        }

        private void OnFlyToggleCanceled(InputAction.CallbackContext ctx)
        {
            foreach (var state in states)
            {
                if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.FLYTOGGLE) && state.ActiveBinds[InputHandler.InputBind.FLYTOGGLE])
                    state.ToggleInputCancelEvent(InputHandler.InputBind.FLYTOGGLE);
            }
        }

        private void OnJumpCanceled(InputAction.CallbackContext ctx)
        {
            foreach (var state in states)
            {
                if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.JUMP) && state.ActiveBinds[InputHandler.InputBind.JUMP])
                    state.ToggleInputCancelEvent(InputHandler.InputBind.JUMP);
            }
        }

        private void OnEquipCanceled(InputAction.CallbackContext ctx)
        {
            foreach (var state in states)
            {
                if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.EQUIP) && state.ActiveBinds[InputHandler.InputBind.EQUIP])
                    state.ToggleInputCancelEvent(InputHandler.InputBind.EQUIP);
            }
        }

        private void OnDropCanceled(InputAction.CallbackContext ctx)
        {
            foreach (var state in states)
            {
                if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.DROP) && state.ActiveBinds[InputHandler.InputBind.DROP])
                    state.ToggleInputCancelEvent(InputHandler.InputBind.DROP);
            }
        }

        private void OnReloadTogglePerfomed(InputAction.CallbackContext ctx)
        {
            foreach (var state in states)
            {
                if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.RELOADTOGGLE) && state.ActiveBinds[InputHandler.InputBind.RELOADTOGGLE])
                    state.ToggleInputEvent(InputHandler.InputBind.RELOADTOGGLE);
            }
        }
        private void OnMouseMidPerfomed(InputAction.CallbackContext ctx)
        {
            foreach (var state in states)
            {
                if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.MOUSEMID) && state.ActiveBinds[InputHandler.InputBind.MOUSEMID])
                    state.ToggleInputEvent(InputHandler.InputBind.MOUSEMID);
            }
        }
        private void OnMouseLeftPerformed(InputAction.CallbackContext ctx)
        {
            foreach (var state in states)
            {
                if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.MOUSELEFT) && state.ActiveBinds[InputHandler.InputBind.MOUSELEFT])
                    state.ToggleInputEvent(InputHandler.InputBind.MOUSELEFT);
            }
        }

        private void OnMouseRightPerformed(InputAction.CallbackContext ctx)
        {
            foreach (var state in states)
            {
                if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.MOUSERIGHT) && state.ActiveBinds[InputHandler.InputBind.MOUSERIGHT])
                    state.ToggleInputEvent(InputHandler.InputBind.MOUSERIGHT);
            }
        }

        private void OnFlyTogglePerformed(InputAction.CallbackContext ctx)
        {
            foreach (var state in states)
            {
                if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.FLYTOGGLE) && state.ActiveBinds[InputHandler.InputBind.FLYTOGGLE])
                    state.ToggleInputEvent(InputHandler.InputBind.FLYTOGGLE);
            }
        }

        private void OnJumpPerformed(InputAction.CallbackContext ctx)
        {
            foreach (var state in states)
            {
                if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.JUMP) && state.ActiveBinds[InputHandler.InputBind.JUMP])
                    state.ToggleInputEvent(InputHandler.InputBind.JUMP);
            }
        }

        private void OnEquipPerformed(InputAction.CallbackContext ctx)
        {
            foreach (var state in states)
            {
                if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.EQUIP) && state.ActiveBinds[InputHandler.InputBind.EQUIP])
                    state.ToggleInputEvent(InputHandler.InputBind.EQUIP);
            }
        }

        private void OnDropPerformed(InputAction.CallbackContext ctx)
        {
            foreach (var state in states)
            {
                if (state.ActiveBinds.ContainsKey(InputHandler.InputBind.DROP) && state.ActiveBinds[InputHandler.InputBind.DROP])
                    state.ToggleInputEvent(InputHandler.InputBind.DROP);
            }
        }

        public void AddState(InputState state)
        {
            if (!states.Contains(state))
                states.Add(state);
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

        private void OnDestroy()
        {
            BlockInput(true);
            states.Clear();
        }
    }
}
