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

        private InputTranslator translator;

        public void Init()
        {
            translator = new InputTranslator();

            VectorBinds.Add(InputBind.WASD, Vector2.zero);
            VectorBinds.Add(InputBind.LOOK, Vector2.zero);
            VectorBinds.Add(InputBind.MOUSEWHEEL, Vector2.zero);

            Binds.Add(InputBind.DROP, translator.Gameplay.Drop);
            Binds.Add(InputBind.EQUIP, translator.Gameplay.Equip);
            Binds.Add(InputBind.JUMP, translator.Gameplay.Jump);

            BoolBinds.Add(InputBind.RUNSTATE, () => translator.Gameplay.Run.IsPressed());
        }

        private void Update()
        {
            VectorBinds[InputBind.WASD] = translator.Gameplay.Movement.ReadValue<Vector2>();
            VectorBinds[InputBind.LOOK] = translator.Gameplay.Camera.ReadValue<Vector2>();
            VectorBinds[InputBind.MOUSEWHEEL] = translator.Gameplay.MouseScrollWheel.ReadValue<Vector2>();
        }

        public enum InputBind : byte
        {
            MOUSELEFT,
            MOUSERIGHT,
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