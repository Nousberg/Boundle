using UnityEngine.InputSystem;
using UnityEngine;
using System;

public class GameInputManager : MonoBehaviour
{
    public event Action<string> OnKeyToggled;
    public event Action OnJump;
    public event Action<Vector2> OnKeyboardInput;
    public event Action<Vector2> OnMouseInput;
    public event Action<Vector3> OnTouchInput;

    private InputTranslator translator;
    private Vector3 direction;

    private void Awake()
    {
        translator = new InputTranslator();
        translator.Enable();
    }
    private void Update()
    {
        direction = translator.Gameplay.Movement.ReadValue<Vector2>();
        if (direction.sqrMagnitude > 0)
            OnKeyboardInput?.Invoke(direction);
    }
    private void OnEnable()
    {
        translator.Gameplay.Jump.performed += JumpPerfomed;
        translator.Gameplay.KeyToggler.performed += KeyToggled;
    }
    private void OnDisable()
    {
        translator.Gameplay.Jump.performed -= JumpPerfomed;
        translator.Gameplay.KeyToggler.performed -= KeyToggled;
    }
    private void JumpPerfomed(InputAction.CallbackContext context)
    {
        OnJump?.Invoke();
    }
    private void KeyToggled(InputAction.CallbackContext context)
    {
        OnKeyToggled?.Invoke(context.control.name);
    }
}
