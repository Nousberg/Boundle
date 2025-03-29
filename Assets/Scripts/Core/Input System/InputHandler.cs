using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Core.InputSystem
{
    public class InputHandler : MonoBehaviour
    {
        public bool mobileInput;

        public Dictionary<InputBind, Vector2> VectorBinds { get; private set; } = new Dictionary<InputBind, Vector2>();
        public Dictionary<InputBind, Func<bool>> BoolBinds { get; private set; } = new Dictionary<InputBind, Func<bool>>();
        public Dictionary<InputBind, InputAction> Binds { get; private set; } = new Dictionary<InputBind, InputAction>();

        private InputSourceV2 source;

        public void Init()
        {
            source = new InputSourceV2();

            source.Gameplay.Enable();

            VectorBinds.Add(InputBind.WASD, Vector2.zero);
            VectorBinds.Add(InputBind.LOOK, Vector2.zero);
            VectorBinds.Add(InputBind.MOUSEWHEEL, Vector2.zero);

            Binds.Add(InputBind.RELOADTOGGLE, source.Gameplay.ReloadToggle);
            Binds.Add(InputBind.MOUSEMID, source.Gameplay.MouseMid);
            Binds.Add(InputBind.MOUSELEFT, source.Gameplay.MouseLeft);
            Binds.Add(InputBind.MOUSERIGHT, source.Gameplay.MouseRight);
            Binds.Add(InputBind.FLYTOGGLE, source.Gameplay.FlyToggle);
            Binds.Add(InputBind.DROP, source.Gameplay.Drop);
            Binds.Add(InputBind.EQUIP, source.Gameplay.Equip);
            Binds.Add(InputBind.JUMP, source.Gameplay.Jump);

            BoolBinds.Add(InputBind.RELOADTOGGLE, () => source.Gameplay.ReloadToggle.IsPressed());
            BoolBinds.Add(InputBind.MOUSEMID, () => source.Gameplay.MouseMid.IsPressed());
            BoolBinds.Add(InputBind.MOUSELEFT, () => source.Gameplay.MouseLeft.IsPressed());
            BoolBinds.Add(InputBind.MOUSERIGHT, () => source.Gameplay.MouseRight.IsPressed());
            BoolBinds.Add(InputBind.FLYTOGGLE, () => source.Gameplay.FlyToggle.IsPressed());
            BoolBinds.Add(InputBind.RUNSTATE, () => source.Gameplay.Run.IsPressed());
        }

        private void Update()
        {
            VectorBinds[InputBind.WASD] = source.Gameplay.Movement.ReadValue<Vector2>();
            VectorBinds[InputBind.LOOK] = source.Gameplay.Look.ReadValue<Vector2>();
            VectorBinds[InputBind.MOUSEWHEEL] = source.Gameplay.MouseWheel.ReadValue<Vector2>();
        }

        public enum InputBind : byte
        {
            MOUSEMID,
            MOUSELEFT,
            MOUSERIGHT,
            RELOADTOGGLE,
            FLYTOGGLE,
            RUNSTATE,
            EQUIP,
            DROP,
            JUMP,
            WASD,
            LOOK,
            MOUSEWHEEL
        }
    }
}