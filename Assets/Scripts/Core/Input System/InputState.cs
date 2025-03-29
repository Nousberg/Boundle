using Assets.Scripts.Core.InputSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.Input_System
{
    public abstract class InputState
    {
        public Dictionary<InputHandler.InputBind, bool> ActiveBinds = new Dictionary<InputHandler.InputBind, bool>();
        public Dictionary<InputHandler.InputBind, bool> BoolBinds = new Dictionary<InputHandler.InputBind, bool>();
        public Dictionary<InputHandler.InputBind, Vector2> VectorBinds = new Dictionary<InputHandler.InputBind, Vector2>();

        public event Action<InputHandler.InputBind> InputRecieved;
        public event Action<InputHandler.InputBind> InputCanceled;

        public void ToggleInputCancelEvent(InputHandler.InputBind type) => InputCanceled?.Invoke(type);
        public void ToggleInputEvent(InputHandler.InputBind type) => InputRecieved?.Invoke(type);
    }
}
